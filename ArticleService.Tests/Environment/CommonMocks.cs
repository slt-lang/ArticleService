using ArticleService.Adapters.Database;
using ArticleService.Domain;
using ArticleService.Domain.Ports;

namespace ArticleService.Tests.Environment
{
    public static class CommonMocks
    {
        public static IArticleDb ArticleDb => new ArticleDb(ArticleContext, Config);
        public static ArticleServiceContext ArticleContext => Extensions.GetInMemoryOptions<ArticleServiceContext>().GetArticleServiceContext();
        public static Config Config => new()
        {
            ArticleHistoryMaxCount = Constants.ArticleHistoryMaxCount,
        };
    }
}
