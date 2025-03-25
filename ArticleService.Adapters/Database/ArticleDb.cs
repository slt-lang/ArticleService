using ArticleService.Adapters.Database.Models;
using ArticleService.Domain;
using ArticleService.Domain.Ports;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using sltlang.Common.ArticleService.Models;

namespace ArticleService.Adapters.Database
{
    public class ArticleDb(IDateTime dateTime, IMemoryCache memoryCache, ArticleServiceContext db, Config config) : IArticleDb
    {
        public async Task<ArticleDto?> GetArticle(string name, bool save_visit = true)
        {
            var articleInfo = await db.Articles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
            if (articleInfo == null)
            {
                return null!;
            }
            var articleContent = await db.ArticleHistory.AsNoTracking().Where(x => x.ArticleId == articleInfo.Id).OrderByDescending(x => x.CreateDate).OrderByDescending(x => x.Id).FirstAsync();

            if (save_visit)
            {
                await AddVisit(articleInfo.Id);
            }

            return new ArticleDto()
            {
                Id = articleInfo.Id,
                HistoryId = articleContent.Id,
                CreateDate = articleInfo.CreateDate,
                UpdateDate = articleContent.CreateDate,
                Name = articleInfo.Name,
                Title = articleInfo.Title,
                Content = articleContent.Content,
            };
        }

        public async Task UpsertArticle(ArticleDto article)
        {
            var existedArticle = await db.Articles.FirstOrDefaultAsync(x => x.Name == article.Name);

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
                });

                await db.SaveChangesAsync();
                await CheckMaxHistory(existedArticle.Id);
            }
            else
            {
                await db.Articles.AddAsync(new Article()
                {
                    Name = article.Name,
                    Title = article.Title,
                    History = [
                        new()
                        {
                            Content = article.Content,
                        }
                    ]
                });
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<ArticleDto>> GetArticleHistory(ArticleDto article)
        {
            var existedArticle = await GetArticle(article.Name);
            if (existedArticle == null)
                throw new ArgumentException(nameof(article.Name));
            var history = await db.ArticleHistory.Where(x => x.ArticleId == existedArticle.Id).ToListAsync();

            return history.Select(x => new ArticleDto()
            {
                Id = article.Id,
                Name = article.Name,
                CreateDate = article.CreateDate,
                Title = article.Title,
                HistoryId = x.Id,
                Content = x.Content,
                UpdateDate = x.CreateDate,
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

        private async Task AddVisit(int aricleId)
        {
            var date = dateTime.UtcNow.Date;

            var vo = await db.ArticleVisits.FindAsync(aricleId, date);

            if (vo == null)
            {
                await db.ArticleVisits.AddAsync(new ArticleVisits()
                {
                    ArticleId = aricleId,
                    Visits = 1,
                    Date = date,
                });
            }
            else
            {
                vo.Visits += 1;
            }

            await db.SaveChangesAsync();
        }

        public async Task<List<ArticleDto>> GetRating(int take = 100)
        {
            if (take > 100) take = 100; if (take < 1) take = 1;

            return (await memoryCache.GetOrCreateAsync("ARTICLE_RATING" + take, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                var topVis = await db.ArticleVisits
                    .GroupBy(x => x.ArticleId)
                    .OrderByDescending(x => x.Sum(y => y.Visits))
                    .Take(take)
                    .Select(x => new { x.Key, Sum = x.Sum(y => y.Visits) }).ToDictionaryAsync(x => x.Key, x => x.Sum);

                var topIds = topVis.Keys.ToArray();

                var topArticles = await db.Articles.Where(x => topIds.Contains(x.Id)).ToListAsync();

                return topArticles.Select(x => new ArticleDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreateDate = x.CreateDate,
                    Title = x.Title,
                    Visits = topVis[x.Id],
                }).ToList();
            }))!;
        }
    }
}
