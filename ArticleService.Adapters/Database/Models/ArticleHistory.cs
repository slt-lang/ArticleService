using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleService.Adapters.Database.Models
{
    public class ArticleHistory
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }
        public string Content { get; set; } = default!;
    }
}
