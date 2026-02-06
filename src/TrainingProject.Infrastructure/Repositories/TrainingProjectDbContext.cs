using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Microsoft.Extensions.Options;
using TrainingProject.Infrastructure.Configuration;

namespace TrainingProject.Infrastructure.Repositories;

public class TrainingProjectDbContext
{
    private readonly INamedBucketProvider _bucketProvider;
    private readonly CouchbaseSettings _settings;
    private IBucket? _bucket;

    public TrainingProjectDbContext(
        INamedBucketProvider bucketProvider,
        IOptions<CouchbaseSettings> options)
    {
        _bucketProvider = bucketProvider;
        _settings = options.Value;
    }

    public async Task<IBucket> GetBucketAsync()
    {
        _bucket ??= await _bucketProvider.GetBucketAsync();

        return _bucket;
    }

    public async Task<ICouchbaseCollection> GetCollectionAsync(string collectionName)
    {
        var bucket = await GetBucketAsync();
        var scope = bucket.Scope(_settings.ScopeName);

        return scope.Collection(collectionName);
    }
}
