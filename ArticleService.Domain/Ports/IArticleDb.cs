using sltlang.Common.ArticleService.Models;

namespace ArticleService.Domain.Ports
{
    public interface IArticleDb
    {
        /// <summary>
        /// Возвращает страницу
        /// </summary>
        Task<ArticleDto?> GetArticle(string name);
        /// <summary>
        /// Создаёт или обновляет страницу
        /// </summary>
        Task UpsertArticle(ArticleDto article);
        /// <summary>
        /// Получает историю изменений страницы
        /// </summary>
        Task<List<ArticleDto>> GetArticleHistory(ArticleDto article);
        /// <summary>
        /// Обновляет страницу до другого состояния
        /// </summary>
        Task RebaseArticle(int historyId);
        /// <summary>
        /// Полностью удаляет страницу
        /// </summary>
        Task DeleteArticle(ArticleDto article);
        /// <summary>
        /// Удаляет историю изменений дальше старше определённого изменения
        /// </summary>
        Task CheckMaxHistory(int articleId);
    }
}
