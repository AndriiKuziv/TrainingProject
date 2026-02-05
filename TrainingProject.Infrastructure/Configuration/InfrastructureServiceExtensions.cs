using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrainingProject.Domain.Repositories;
using TrainingProject.Infrastructure.Repositories;

namespace TrainingProject.Infrastructure.Configuration;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var couchbaseSettings = configuration.GetSection("Couchbase").Get<CouchbaseSettings>();

        services
            .AddCouchbase(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("couchbase");
                options.UserName = configuration["Couchbase:UserName"];
                options.Password = configuration["Couchbase:Password"];
            })
            .AddCouchbaseBucket<INamedBucketProvider>(couchbaseSettings.BucketName ?? "training-bucket");

        services.AddScoped<TrainingProjectDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
