using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TrainingProject.Application.Requests;

namespace TrainingProject.Application.Validation;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");
    }
}
