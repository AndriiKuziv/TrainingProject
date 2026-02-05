using Couchbase.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using TrainingProject.Api.Middleware;
using TrainingProject.Application.Configuration;
using TrainingProject.Infrastructure.Configuration;

namespace TrainingProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddLogging();

            builder.Services.Configure<CouchbaseSettings>(builder.Configuration.GetSection("Couchbase"));
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
            builder.Services.AddProblemDetails();

            builder.Services.AddOpenApiDocument(settings =>
            {
                settings.Title = "TrainingProject API";
                settings.Version = "v1";
            });

            var app = builder.Build();

            var cluster = app.Services.GetRequiredService<IClusterProvider>().GetClusterAsync().GetAwaiter().GetResult();
            var timeoutDuration = TimeSpan.FromSeconds(60);
            cluster.WaitUntilReadyAsync(timeoutDuration).GetAwaiter().GetResult();

            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
