using ArticleService.Adapters.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Adapters.Database
{
    public class ArticleServiceContext(DbContextOptions options) : DbContext(options)
    {
        public const string ArticleScheme = "ArticleService";

        public DbSet<Article> Articles { get; set; } = default!;
        public DbSet<ArticleHistory> ArticleHistory { get; set; } = default!;
        public DbSet<ArticleVisits> ArticleVisits { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(e =>
            {
                e.ToTable(nameof(Articles), ArticleScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.Name).IsRequired().HasMaxLength(64);
                e.Property(e => e.CultureKey).IsRequired(false).HasMaxLength(16);
                e.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql(DateTimeNowSqlFunction);
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
                e.Property(e => e.CreateDate).IsRequired().HasDefaultValueSql(DateTimeNowSqlFunction);
                e.Property(e => e.Content).IsRequired().IsUnicode(true);

                e.HasIndex(e => e.Id);
                e.HasIndex(e => e.ArticleId);
                e.HasIndex(e => e.CreateDate);
            });

            modelBuilder.Entity<ArticleVisits>(e =>
            {
                e.ToTable(nameof(ArticleVisits), ArticleScheme);

                e.HasKey(e => new { e.ArticleId, e.Date });
                e.Property(e => e.Visits).IsRequired();
                e.Property(e => e.ArticleId).IsRequired();
                e.Property(e => e.Date).IsRequired();

                e.HasOne(e => e.Article).WithMany().OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(e => e.Date);
                e.HasIndex(e => e.ArticleId);
            });
        }

        private string DateTimeNowSqlFunction
        {
            get
            {
                if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
                    return "NOW()";
                return "GETDATE()";
            }
        }
    }
}
