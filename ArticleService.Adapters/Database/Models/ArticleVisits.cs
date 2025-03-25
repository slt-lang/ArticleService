namespace ArticleService.Adapters.Database.Models
{
    public class ArticleVisits
    {
        public int ArticleId { get; set; }
        public DateTime Date { get; set; }
        public long Visits { get; set; }

        public Article? Article { get; set; } = default!;
    }
}
