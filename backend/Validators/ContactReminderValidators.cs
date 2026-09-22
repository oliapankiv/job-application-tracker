using FluentValidation;
using JobTracker.Api.DTOs;

namespace JobTracker.Api.Validators;

public class CreateContactDtoValidator : AbstractValidator<CreateContactDto>
{
    public CreateContactDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

public class CreateReminderDtoValidator : AbstractValidator<CreateReminderDto>
{
    public CreateReminderDtoValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DueDate).NotEmpty();
    }
}

public class UpdateReminderDtoValidator : AbstractValidator<UpdateReminderDto>
{
    public UpdateReminderDtoValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DueDate).NotEmpty();
    }
}
