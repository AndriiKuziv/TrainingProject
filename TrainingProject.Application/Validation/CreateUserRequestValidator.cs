using FluentValidation;
using TrainingProject.Application.Requests;

namespace TrainingProject.Application.Validation;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotNull().NotEmpty().WithMessage("Name is required");
    }
}
