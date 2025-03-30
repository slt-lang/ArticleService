using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleService.Adapters.Database.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? CultureKey { get; set; } = default!;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; } = default!;
        public string Title { get; set; } = default!;

        public virtual ICollection<ArticleHistory> History { get; set; } = default!;
    }
}
