using ArticleService.Adapters.Database;
using ArticleService.Domain;
using ArticleService.Domain.Logic;
using ArticleService.Domain.Ports;
using Microsoft.Extensions.Caching.Memory;

namespace ArticleService.Tests.Environment
{
    public static class CommonMocks
    {
        public static IDateTime DateTimeProvider = new DateTimeProvider();
        public static IArticleDb ArticleDb => new ArticleDb(DateTimeProvider, MemoryCache, ArticleContext, Config);
        public static ArticleServiceContext ArticleContext => Extensions.GetInMemoryOptions<ArticleServiceContext>().GetArticleServiceContext();
        public static Config Config => new()
        {
            ArticleHistoryMaxCount = Constants.ArticleHistoryMaxCount,
        };

        public static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
    }
}
