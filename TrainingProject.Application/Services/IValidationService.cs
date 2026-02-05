using FluentValidation.Results;

namespace TrainingProject.Application.Services;

public interface IValidationService
{
    Task ValidateAsync<T>(T model, CancellationToken cancellationToken = default);

    void ValidateNull<T>(T model);
}
