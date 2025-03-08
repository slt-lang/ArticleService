using ArticleService.Adapters.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Adapters.Database
{
    public class ArticleServiceContext(DbContextOptions options) : DbContext(options)
    {
        public const string ArticleScheme = "ArticleService";

        public DbSet<Article> Articles { get; set; } = default!;
        public DbSet<ArticleHistory> ArticleHistory { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(e =>
            {
                e.ToTable(nameof(Articles), ArticleScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.Name).IsRequired().HasMaxLength(64);
                e.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("NOW()");
                e.Property(e => e.Title).IsRequired().HasMaxLength(128).IsUnicode(true);

                e.HasMany(e => e.History).WithOne().HasForeignKey(e => e.ArticleId).OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(e => e.Id);
                e.HasIndex(e => e.Name);
                e.HasIndex(e => e.CreateDate);
            });

            modelBuilder.Entity<ArticleHistory>(e =>
            {
                e.ToTable(nameof(ArticleHistory), ArticleScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql("NOW()");
                e.Property(e => e.Content).IsRequired().IsUnicode(true);

                e.HasIndex(e => e.Id);
                e.HasIndex(e => e.ArticleId);
                e.HasIndex(e => e.CreateDate);
            });
        }
    }
}
