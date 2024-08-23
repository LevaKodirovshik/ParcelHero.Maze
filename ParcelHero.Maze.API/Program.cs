using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace ParcelHero.Maze.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IPathfindingCache, PathfindingCache>();
            builder.Services.AddKeyedScoped<IPathfindingAlgorithm, BasicPathfindingAlgorithm>(PathfindingAlgorithmName.Basic);
            builder.Services.AddScoped<IPathfindingAlgorithmProvider, PathfindingAlgorithmProvider>();

            // Add JSON enum converter so that PathfindingAlgorithmName is displayed as a string in swagger
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // add formatter to allow plain text
            builder.Services.AddControllers(o => o.InputFormatters.Add(new PlainTextInputFormatter()));

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
