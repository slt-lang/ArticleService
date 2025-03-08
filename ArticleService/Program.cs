using ArticleService.Adapters.Database;
using ArticleService.Domain;
using ArticleService.Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace ArticleService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ArticleDb");
            if (connectionString == null)
                connectionString = builder.Configuration["DB_CONNECTION_STRING"];

            builder.Services.AddNpgsql<ArticleServiceContext>(connectionString);
            builder.Services.AddTransient<IArticleDb, ArticleDb>();

            var configuration = builder.Configuration.GetSection("Config").Get<Config>();
            builder.Services.AddSingleton(configuration!);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ArticleServiceContext>();
                await db.Database.MigrateAsync();
            }

            app.Run();
        }
    }
}
