using Moq;
using TrainingProject.Application.Requests;
using TrainingProject.Application.Services;
using TrainingProject.Application.Services.Implementations;
using TrainingProject.Application.Tests.Unit.Helpers;
using TrainingProject.Domain.Models;
using TrainingProject.Domain.Repositories;

namespace TrainingProject.Application.Tests.Unit.Services.Implementations;

public class UserServiceTests : IClassFixture<AutoMapperFixture>
{
    private readonly AutoMapperFixture _mapperFixture;
    private readonly Mock<IValidationService> _validationServiceMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public UserServiceTests(AutoMapperFixture mapperFixture)
    {
        _mapperFixture = mapperFixture;
    }

    [Theory]
    [MemberData(nameof(GetAllUsersAsyncData))]
    public async Task GetAllUsersAsync_ReturnsMappedDtos(IEnumerable<User> users)
    {
        // Arrange
        _userRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var userService = CreateUserService();

        // Act
        var result = await userService.GetAllUsersAsync();
        var userDtos = result.ToList();

        // Assert
        Assert.Equal(users.Count(), userDtos.Count);
        foreach (var expected in users)
        {
            Assert.Contains(result, actual => actual.Id == expected.Id && actual.Name == expected.Name);
        }

        _userRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("id-1", "User1")]
    [InlineData("id-2", "User2")]
    public async Task GetUserByIdAsync_ValidData_ReturnsMappedDto(string userId, string name)
    {
        // Arrange
        var user = new User { Id = userId, Name = name };

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var userService = CreateUserService();

        // Act
        var result = await userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Equal(userId, result.Id);
        Assert.Equal(name, result.Name);

        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("User1", "id-1")]
    [InlineData("User2", "id-2")]
    public async Task CreateUserAsync_ValidData_ReturnsCreatedUserId(string name, string generatedId)
    {
        // Arrange
        var request = new CreateUserRequest { Name = name };

        _validationServiceMock
            .Setup(validation => validation.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(repo =>
                repo.CreateAsync(It.Is<User>(u => u.Name == name), It.IsAny<CancellationToken>()))
            .ReturnsAsync(generatedId);

        var userService = CreateUserService();

        // Act
        var result = await userService.CreateUserAsync(request);

        // Assert
        Assert.Equal(generatedId, result);

        _validationServiceMock.Verify(validation => validation.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(
            repo => repo.CreateAsync(It.Is<User>(u => u.Name == name), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("id-1", "User1", "=UpdatedUser1")]
    [InlineData("id-2", "User2", "!UpdatedUser2")]
    public async Task UpdateUserAsync_ValidData_ReturnsTrue(
        string userId,
        string existingName,
        string newName)
    {
        // Arrange
        var request = new UpdateUserRequest { Name = newName };
        var existingUser = new User { Id = userId, Name = existingName };

        _validationServiceMock
            .Setup(validation => validation.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                userId,
                It.Is<User>(u => u.Name == newName),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var userService = CreateUserService();

        // Act
        var result = await userService.UpdateUserAsync(userId, request);

        // Assert
        Assert.True(result);

        _validationServiceMock.Verify(validation => validation.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(
            repo => repo.UpdateAsync(userId, It.Is<User>(u => u.Name == newName), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("id-1")]
    [InlineData("id-2")]
    public async Task DeleteUserAsync_ValidData_ReturnsTrue(string userId)
    {
        // Arrange
        _userRepositoryMock
            .Setup(repo => repo.DeleteAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var userService = CreateUserService();

        // Act
        var result = await userService.DeleteUserAsync(userId);

        // Assert
        Assert.True(result);

        _userRepositoryMock.Verify(repo => repo.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private UserService CreateUserService() => new(
        _validationServiceMock.Object,
        _userRepositoryMock.Object,
        _mapperFixture.Mapper);

    public static IEnumerable<object[]> GetAllUsersAsyncData()
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
}
