using ArticleService.Adapters.Database;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Tests.Environment
{
    internal static class Extensions
    {
        public static ArticleServiceContext GetArticleServiceContext(this DbContextOptions<ArticleServiceContext> options)
        {
            var context = new ArticleServiceContext(options);
            context.Database.EnsureCreated();
            context.Database.EnsureDeleted();
            return context;
        }

        public static DbContextOptions<T> GetInMemoryOptions<T>() where T : DbContext
        {
            return new DbContextOptionsBuilder<T>().UseInMemoryDatabase(databaseName: "Fake" + typeof(T).Name).Options;
        }
    }
}
