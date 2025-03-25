using ArticleService.Adapters.Database;
using ArticleService.Tests.Environment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using sltlang.Common.ArticleService.Models;
using System.Xml;

namespace ArticleService.Tests
{
    public class ArticleTests
    {
        public static string ArticleName => "Test";
        public static ArticleDto NewArticle => new ArticleDto()
        {
            Content = "Content",
            Name = ArticleName,
            Title = "TestTitle",
        };

        [Fact]
        public async void SaveAndGet()
        {
            var articleService = CommonMocks.ArticleDb;

            await articleService.UpsertArticle(NewArticle);

            var dto = await articleService.GetArticle(ArticleName);

            Assert.NotNull(dto);
            Assert.True(NewArticle.Content == dto.Content);
        }

        [Fact]
        public async void MaxHistory()
        {
            var articleDbContext = CommonMocks.ArticleContext;
            var config = CommonMocks.Config;
            var articleService = new ArticleDb(articleDbContext, config);

            for (var i = 0; i < config.ArticleHistoryMaxCount * 2; i++)
            {
                await articleService.UpsertArticle(NewArticle);
            }

            Assert.Equal(config.ArticleHistoryMaxCount, await articleDbContext.ArticleHistory.CountAsync());
        }

        [Fact]
        public async void Rebase()
        {
            var articleDbContext = CommonMocks.ArticleContext;
            var config = CommonMocks.Config;
            var articleService = new ArticleDb(articleDbContext, config);

            for (var i = 0; i < 10; i++)
            {
                await articleService.UpsertArticle(NewArticle);
            }

            var r = Random.Shared.Next().ToString("X");

            {
                var not_new_article = NewArticle;
                not_new_article.Content = r;
                await articleService.UpsertArticle(not_new_article);
            }

            var hid = default(int);
            {
                var dto = await articleService.GetArticle(ArticleName);
                Assert.NotNull(dto);
                hid = dto.HistoryId;
            }

            for (var i = 0; i < 10; i++)
            {
                await articleService.UpsertArticle(NewArticle);
            }

            await articleService.RebaseArticle(hid);

            {
                var dto = await articleService.GetArticle(ArticleName);
                Assert.NotNull(dto);
                Assert.Equal(r, dto.Content);
            }

        }
    }
}