using Couchbase.KeyValue;
using Couchbase.KeyValue.RangeScan;
using Microsoft.Extensions.Options;
using TrainingProject.Domain.Models;
using TrainingProject.Domain.Repositories;
using TrainingProject.Infrastructure.Configuration;

namespace TrainingProject.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ICouchbaseCollection _collection;

    public UserRepository(
        TrainingProjectDbContext dbContext,
        IOptions<CouchbaseSettings> options)
    {
        var settings = options.Value;

        _collection = dbContext
            .GetCollectionAsync(settings.Collections["UsersCollection"])
            .GetAwaiter()
            .GetResult();
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = new List<User>();
        var scanResult = _collection.ScanAsync(new RangeScan());

        await foreach (var doc in scanResult)
        {
            users.Add(doc.ContentAs<User>());
        }
        
        return users;
    }

    public async Task<User> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var doc = await _collection.GetAsync(userId);

        return doc.ContentAs<User>()!;
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var userId = Guid.NewGuid().ToString();
        user.Id = userId;

        await _collection.InsertAsync(userId, user);

        return user;
    }

    public async Task<User> UpdateAsync(string userId, User user, CancellationToken cancellationToken = default)
    {
        user.Id = userId;

        await _collection.ReplaceAsync(userId, user);

        return user;
    }

    public async Task<User> DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var doc = await _collection.GetAsync(userId);
        var user = doc.ContentAs<User>()!;

        await _collection.RemoveAsync(userId);

        return user;
    }
}
