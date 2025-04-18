using System.Xml.Linq;
using ArticleService.Adapters.Database.Models;
using ArticleService.Domain;
using ArticleService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using sltlang.Common.ArticleService.Models;

namespace ArticleService.Adapters.Database
{
    public class ArticleDb(IDateTime dateTime, IMemoryCache memoryCache, ArticleServiceContext db, Config config) : IArticleDb
    {
        public async Task<ArticleDto?> GetArticle(string? culture_key, string name, bool save_visit = true)
        {
            var articleInfo = await db.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name && x.CultureKey == culture_key);
            if (articleInfo == null)
            {
                return null!;
            }
            var articleContent = await db.ArticleHistory.AsNoTracking().Where(x => x.ArticleId == articleInfo.Id).OrderByDescending(x => x.CreateDate).OrderByDescending(x => x.Id).FirstAsync();
            var visitsWeek = await db.ArticleVisits.AsNoTracking().Where(x => x.ArticleId == articleInfo.Id && x.Date >= dateTime.UtcToday.AddDays(-7)).SumAsync(x => x.Visits);
            var visits = await db.ArticleVisits.AsNoTracking().Where(x => x.ArticleId == articleInfo.Id).SumAsync(x => x.Visits);

            if (save_visit)
            {
                await AddVisit(articleInfo.Id);
            }

            return new ArticleDto()
            {
                Id = articleInfo.Id,
                CultureKey = articleInfo.CultureKey!,
                HistoryId = articleContent.Id,
                CreateDate = articleInfo.CreateDate,
                UpdateDate = articleContent.CreateDate,
                Name = articleInfo.Name,
                Title = articleInfo.Title,
                Content = articleContent.Content,
                Visits = visits,
                VisitsWeekly = visitsWeek,
                UserId = articleContent.UserId,
            };
        }

        public async Task UpsertArticle(ArticleDto article)
        {
            var existedArticle = await db.Articles.FirstOrDefaultAsync(x => x.Name == article.Name && x.CultureKey == article.CultureKey);

            if (existedArticle != null)
            {
                if (existedArticle.Title != article.Title)
                {
                    existedArticle.Title = article.Title;
                }

                await db.ArticleHistory.AddAsync(new ArticleHistory()
                {
                    ArticleId = existedArticle.Id,
                    Content = article.Content,
                    UserId = article.UserId,
                });

                await db.SaveChangesAsync();
                await CheckMaxHistory(existedArticle.Id);
                if (article.Visits != default)
                    await AddVisit(existedArticle.Id, article.Visits);
            }
            else
            {
                var newArticle = await db.Articles.AddAsync(new Article()
                {
                    CultureKey = article.CultureKey,
                    Name = article.Name,
                    Title = article.Title,
                    History = [
                        new()
                        {
                            Content = article.Content,
                            UserId = article.UserId,
                        }
                    ],
                });
                await db.SaveChangesAsync();
                await AddVisit(newArticle.Entity.Id, article.Visits);
            }
        }

        public async Task<List<ArticleDto>> GetArticleHistory(ArticleDto article)
        {
            var existedArticle = await GetArticle(article.CultureKey, article.Name);
            if (existedArticle == null)
                throw new ArgumentException(nameof(article.Name));
            var history = await db.ArticleHistory.Where(x => x.ArticleId == existedArticle.Id).ToListAsync();

            return history.Select(x => new ArticleDto()
            {
                Id = article.Id,
                CultureKey = article.CultureKey,
                Name = article.Name,
                CreateDate = article.CreateDate,
                Title = article.Title,
                HistoryId = x.Id,
                Content = x.Content,
                UpdateDate = x.CreateDate,
                UserId = x.UserId,
            }).ToList();
        }

        public async Task RebaseArticle(int historyId)
        {
            var articleContent = await db.ArticleHistory.Where(x => x.Id == historyId).FirstOrDefaultAsync();
            if (articleContent == null)
                throw new ArgumentException(nameof(historyId));
            await db.ArticleHistory.AddAsync(new ArticleHistory()
            {
                ArticleId = articleContent.ArticleId,
                Content = articleContent.Content,
                UserId= articleContent.UserId,
            });
            await CheckMaxHistory(articleContent.ArticleId);
            await db.SaveChangesAsync();
        }

        public async Task DeleteArticle(ArticleDto article)
        {
            var articleObject = await db.Articles.FindAsync(article.Id);
            if (articleObject == null)
                throw new ArgumentException(nameof(article.Name));
            db.Articles.Remove(articleObject);
            await db.SaveChangesAsync();
        }

        public async Task CheckMaxHistory(int articleId)
        {
            var count = db.ArticleHistory.Where(x => x.ArticleId == articleId).Count();
            if (count > config.ArticleHistoryMaxCount)
            {
                foreach (var articleHistory in await db.ArticleHistory.Where(x => x.ArticleId == articleId).OrderByDescending(x => x.CreateDate).Skip(config.ArticleHistoryMaxCount).ToListAsync())
                {
                    db.ArticleHistory.Remove(articleHistory);
                }
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<ArticleDto>> GetArticlesByUserId(int userId)
        {
            var articlesHistories = await db.ArticleHistory.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreateDate)
                .Select(x => new { x.ArticleId, x.CreateDate })
                .ToArrayAsync();

            var articleIds = articlesHistories.Select(x => x.ArticleId).Distinct().ToArray();

            var articleInfo = await db.Articles.AsNoTracking().Where(x => articleIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x);

            return articlesHistories.DistinctBy(x => x.ArticleId).Select(x => new ArticleDto()
            {
                Id = x.ArticleId,
                CultureKey = articleInfo[x.ArticleId].CultureKey!,
                UpdateDate = x.CreateDate,
                Name = articleInfo[x.ArticleId].Name,
                Title = articleInfo[x.ArticleId].Title,
                UserId = userId,
            }).ToList();
        }

        private async Task AddVisit(int aricleId, long increment = 1)
        {
            var date = dateTime.UtcToday;

            var vo = await db.ArticleVisits.FindAsync(aricleId, date);

            if (vo == null)
            {
                await db.ArticleVisits.AddAsync(new ArticleVisits()
                {
                    ArticleId = aricleId,
                    Visits = increment,
                    Date = date,
                });
            }
            else
            {
                vo.Visits += increment;
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<ArticleDto>> GetRating(string? cultureKey, int take = 100)
        {
            if (take > 100) take = 100; if (take < 1) take = 1;

            return (await memoryCache.GetOrCreateAsync("ARTICLE_RATING" + $"{cultureKey??"null"}/{take}" , async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                var date = dateTime.UtcToday.AddDays(-7);

                var topVis = await db.ArticleVisits
                    .Where(x => x.Article!.CultureKey == cultureKey)
                    .GroupBy(x => x.ArticleId)
                    .OrderByDescending(x => x.Sum(y => y.Visits))
                    .Take(take)
                    .Select(x => new { x.Key, Sum = x.Sum(y => y.Visits), Sum7Days = x.Where(x => x.Date >= date).Sum(y => y.Visits) }).ToDictionaryAsync(x => x.Key, x => (x.Sum, x.Sum7Days));

                var topIds = topVis.Keys.ToArray();

                var topArticles = await db.Articles.Where(x => topIds.Contains(x.Id)).ToListAsync();

                return topArticles.Select(x => new ArticleDto
                {
                    Id = x.Id,
                    CultureKey = x.CultureKey!,
                    Name = x.Name,
                    CreateDate = x.CreateDate,
                    Title = x.Title,
                    Visits = topVis[x.Id].Sum,
                    VisitsWeekly = topVis[x.Id].Sum7Days,
                }).ToList();
            }))!;
        }
    }
}
