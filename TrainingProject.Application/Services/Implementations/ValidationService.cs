using FluentValidation;
using FluentValidation.Results;
using System.Reflection;

namespace TrainingProject.Application.Services.Implementations;

public class ValidationService : IValidationService
{
    private readonly Dictionary<Type, object> _validators = [];

    public async Task ValidateAsync<T>(T model, CancellationToken cancellationToken = default)
    {
        ValidateNull(model);

        var validatorType = GetValidatorType(model);

        if (!_validators.TryGetValue(validatorType, out _))
        {
            var validatorInstance = Activator.CreateInstance(validatorType)
                ?? throw new KeyNotFoundException("Unable to find validator of the required type");

            _validators[validatorType] = validatorInstance;
        }

        var validationResult = await ((IValidator)_validators[validatorType]).ValidateAsync(new ValidationContext<object>(model), cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException("Failed to validate the model", validationResult.Errors);
        }
    }

    public void ValidateNull<T>(T model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), "Model cannot be null.");
        }
    }

    private static Type GetValidatorType<T>(T model)
    {
        var modelType = model!.GetType();
        var evt = typeof(AbstractValidator<>).MakeGenericType(modelType);

        return Assembly.GetAssembly(typeof(ValidationService))!
            .GetTypes()
            .First(t => t.IsSubclassOf(evt));
    }
}
