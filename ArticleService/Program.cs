using ArticleService.Adapters.Database;
using ArticleService.Domain;
using ArticleService.Domain.Logic;
using ArticleService.Domain.Ports;
using Microsoft.EntityFrameworkCore;
using sltlang.Common.Common;

namespace ArticleService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddNpgsql<ArticleServiceContext>(builder.Configuration.GetConnectionString("ArticleDb"));
            builder.Services.AddTransient<IArticleDb, ArticleDb>();
            builder.Services.AddTransient<IDateTime, DateTimeProvider>();
            builder.Services.AddMemoryCache();

            var configuration = builder.Configuration.GetSection("Config").Get<Config>();
            builder.Services.AddSingleton(configuration!);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapGet("/serviceinfo", () => ServiceInfo.GetServiceInfo(typeof(Program).Assembly)).WithGroupName("Service");

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
