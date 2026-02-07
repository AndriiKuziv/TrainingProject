using FluentValidation;
using TrainingProject.Application.Requests;
using TrainingProject.Application.Services.Implementations;

namespace TrainingProject.Application.Tests.Unit.Services.Implementations;

public class ValidationServiceTests
{
    private readonly ValidationService _validationService = new();

    [Fact]
    public async Task ValidateAsync_ValidModel_PassesValidation()
    {
        // Arrange
        var request = new CreateUserRequest { Name = "User1" };

        // Act
        var exception = await Record.ExceptionAsync(() => _validationService.ValidateAsync(request));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task ValidateAsync_InvalidModel_ThrowsValidationException()
    {
        // Arrange
        var invalidRequest = new CreateUserRequest { Name = string.Empty };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _validationService.ValidateAsync(invalidRequest));
    }

    [Fact]
    public async Task ValidateAsync_NullModel_ThrowsArgumentNullException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _validationService.ValidateAsync<CreateUserRequest>(null));
    }

    [Fact]
    public void ValidateNull_NullModel_ThrowsArgumentNullException()
    {
        // Arrange

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _validationService.ValidateNull<object>(null));
    }

    [Fact]
    public void ValidateNull_NotNullModel_PassesValidation()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => _validationService.ValidateNull("NotNull"));

        // Assert
        Assert.Null(exception);
    }
}
