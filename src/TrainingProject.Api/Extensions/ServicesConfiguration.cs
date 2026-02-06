using Microsoft.Extensions.Configuration;
using Couchbase.Extensions.DependencyInjection;

namespace TrainingProject.Api.Extensions;

public static class ServicesConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddCouchbase(configuration.GetSection("Couchbase"));
        services.AddCouchbaseBucket<INamedBucketProvider>("training-bucket");
    }
}
