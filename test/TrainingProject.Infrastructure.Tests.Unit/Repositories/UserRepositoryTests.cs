using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Couchbase.KeyValue.RangeScan;
using Microsoft.Extensions.Options;
using Moq;
using TrainingProject.Domain.Models;
using TrainingProject.Infrastructure.Configuration;
using TrainingProject.Infrastructure.Repositories;

namespace TrainingProject.Infrastructure.Tests.Unit.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<ICouchbaseCollection> _collectionMock = new();
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var settings = new CouchbaseSettings
        {
            ScopeName = "TestScope",
            Collections = new Dictionary<string, string>
            {
                ["UsersCollection"] = "users"
            }
        };

        var scopeMock = new Mock<IScope>();
        scopeMock
            .Setup(scope => scope.Collection(settings.Collections["UsersCollection"]))
            .Returns(_collectionMock.Object);

        var bucketMock = new Mock<IBucket>();
        bucketMock
            .Setup(bucket => bucket.Scope(settings.ScopeName))
            .Returns(scopeMock.Object);

        var bucketProviderMock = new Mock<INamedBucketProvider>();
        bucketProviderMock
            .Setup(provider => provider.GetBucketAsync())
            .ReturnsAsync(bucketMock.Object);

        var options = Options.Create(settings);
        var dbContext = new TrainingProjectDbContext(bucketProviderMock.Object, options);

        _userRepository = new UserRepository(dbContext, options);
    }

    [Theory]
    [MemberData(nameof(GetAllAsyncData))]
    public async Task GetAllAsync_ReturnsAllUsers(IEnumerable<User> users)
    {
        // Arrange
        var scanResults = users
            .Select(user =>
            {
                var scanResult = new Mock<IScanResult>();
                scanResult.Setup(result => result.ContentAs<User>()).Returns(user);
                return scanResult.Object;
            })
            .ToArray();

        _collectionMock
            .Setup(collection => collection.ScanAsync(It.IsAny<RangeScan>(), It.IsAny<ScanOptions?>()))
            .Returns(CreateScanEnumerable(scanResults));

        // Act
        var result = (await _userRepository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(users.Count(), result.Count);
        foreach (var expected in users)
        {
            Assert.Contains(result, actual => actual.Id == expected.Id && actual.Name == expected.Name);
        }

        _collectionMock.Verify(collection => collection.ScanAsync(It.IsAny<RangeScan>(), It.IsAny<ScanOptions?>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsExistingUser()
    {
        // Arrange
        var existingUser = new User { Id = "id-1", Name = "User1" };

        var getResultMock = new Mock<IGetResult>();
        getResultMock.Setup(result => result.ContentAs<User>()).Returns(existingUser);

        _collectionMock
            .Setup(collection => collection.GetAsync(existingUser.Id, It.IsAny<GetOptions?>()))
            .ReturnsAsync(getResultMock.Object);

        // Act
        var result = await _userRepository.GetByIdAsync(existingUser.Id);

        // Assert
        Assert.Equal(existingUser, result);
        _collectionMock.Verify(collection => collection.GetAsync(existingUser.Id, It.IsAny<GetOptions?>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValidData_CreatesUserAndReturnsUserId()
    {
        // Arrange
        var user = new User { Name = "NewUser" };
        _collectionMock
            .Setup(collection => collection.InsertAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<InsertOptions?>()))
            .ReturnsAsync(Mock.Of<IMutationResult>());

        // Act
        var createdUser = await _userRepository.CreateAsync(user);

        // Assert
        Assert.NotNull(createdUser);
        Assert.False(string.IsNullOrEmpty(createdUser.Id));

        _collectionMock.Verify(
            collection => collection.InsertAsync(createdUser.Id, user, It.IsAny<InsertOptions?>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidData_UpdatesUserAndReturnsTrue()
    {
        // Arrange
        var user = new User { Name = "UpdatedUser" };
        var userId = "id-1";

        _collectionMock
            .Setup(collection => collection.ReplaceAsync(userId, It.IsAny<User>(), It.IsAny<ReplaceOptions?>()))
            .ReturnsAsync(Mock.Of<IMutationResult>());

        // Act
        var updatedUser = await _userRepository.UpdateAsync(userId, user);

        // Assert
        Assert.NotNull(updatedUser);
        Assert.Equal(userId, updatedUser.Id);

        _collectionMock.Verify(
            collection => collection.ReplaceAsync(
                userId,
                It.Is<User>(u => u.Id == userId && u.Name == user.Name),
                It.IsAny<ReplaceOptions?>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidData_RemovesUserAndReturnsDeletedUser()
    {
        // Arrange
        var user = new User { Id = "id-1", Name = "User1" };

        var getResultMock = new Mock<IGetResult>();
        getResultMock.Setup(result => result.ContentAs<User>()).Returns(user);

        _collectionMock
            .Setup(collection => collection.GetAsync(user.Id, It.IsAny<GetOptions?>()))
            .ReturnsAsync(getResultMock.Object);

        _collectionMock
            .Setup(collection => collection.RemoveAsync(user.Id, It.IsAny<RemoveOptions?>()))
            .Returns(Task.CompletedTask);

        // Act
        var deletedUser = await _userRepository.DeleteAsync(user.Id);

        // Assert
        Assert.NotNull(deletedUser);
        Assert.Equal(user.Id, deletedUser.Id);
        Assert.Equal(user.Name, deletedUser.Name);

        _collectionMock.Verify(collection => collection.RemoveAsync(user.Id, It.IsAny<RemoveOptions?>()), Times.Once);
    }

    public static IEnumerable<object[]> GetAllAsyncData()
    {
        yield return new object[]
        {
            new List<User>()
        };

        yield return new object[]
        {
            new List<User>
            {
                new() { Id = "id-1", Name = "User1" }
            }
        };

        yield return new object[]
        {
            new List<User>
            {
                new() { Id = "id-1", Name = "User1" },
                new() { Id = "id-2", Name = "User2" },
                new() { Id = "id-3", Name = "User3" }
            }
        };
    }

    private static IAsyncEnumerable<IScanResult> CreateScanEnumerable(params IScanResult[] results)
    {
        return Iterate(results);

        static async IAsyncEnumerable<IScanResult> Iterate(IEnumerable<IScanResult> entries)
        {
            foreach (var entry in entries)
            {
                yield return entry;
                await Task.Yield();
            }
        }
    }
}
