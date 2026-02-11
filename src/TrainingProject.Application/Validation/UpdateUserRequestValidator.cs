using FluentValidation;
using TrainingProject.Application.Constants;
using TrainingProject.Application.Requests;

namespace TrainingProject.Application.Validation;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().NotEmpty().WithMessage("Name is required.");
    }
}
