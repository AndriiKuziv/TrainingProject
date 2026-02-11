using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrainingProject.Application.Configuration;
using TrainingProject.Application.Requests;
using TrainingProject.Application.Services.Implementations;

namespace TrainingProject.Application.Tests.Unit.Services.Implementations;

public class ValidationServiceTests
{
    private readonly ValidationService _validationService;

    public ValidationServiceTests()
    {
        var services = new ServiceCollection();

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly, includeInternalTypes: true);

        var serviceProvider = services.BuildServiceProvider();
        _validationService = new ValidationService(serviceProvider);
    }

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

        // Act
        var exception = await Record.ExceptionAsync(() => _validationService.ValidateAsync(invalidRequest));
        
        // Assert
        Assert.NotNull(exception);
        var validationException = Assert.IsType<ValidationException>(exception);
        Assert.Contains(validationException!.Errors, errors => errors.ErrorMessage == "Name is required.");
    }

    [Fact]
    public async Task ValidateNull_NullModel_ThrowsArgumentNullException()
    {
        // Arrange
        var errorMessage = "Test error message";

        // Act
        var exception = Record.Exception(() => _validationService.ValidateNull<object>(null, errorMessage));

        // Assert
        Assert.NotNull(exception);
        var validationException = Assert.IsType<ValidationException>(exception);
        Assert.Equal(errorMessage, validationException.Message);
    }

    [Fact]
    public void ValidateNull_NotNullModel_PassesValidation()
    {
        // Arrange
        var validModel = new object();
        var errorMessage = "Test error message";

        // Act
        var exception = Record.Exception(() => _validationService.ValidateNull(validModel, errorMessage));

        // Assert
        Assert.Null(exception);
    }
}
