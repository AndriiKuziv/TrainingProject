using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TrainingProject.Application.Services.Implementations;

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ValidateAsync<T>(T model, CancellationToken cancellationToken = default)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();

        if (validator is null)
        {
            throw new InvalidOperationException($"Failed to find validator for the model type {typeof(T).Name}.");
        }

        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Failed to validate the model", validationResult.Errors);
        }
    }

    public void ValidateNull<T>(T model, string errorMessage, params object[] args)
    {
        if (model is null)
        {
            throw new ValidationException(string.Format(errorMessage, args));
        }
    }
}
