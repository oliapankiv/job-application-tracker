using FluentValidation;
using JobTracker.Api.DTOs;

namespace JobTracker.Api.Validators;

public class CreateApplicationDtoValidator : AbstractValidator<CreateApplicationDto>
{
    public CreateApplicationDtoValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobPostingUrl).MaximumLength(2048);
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(0).When(x => x.Salary.HasValue);
    }
}

public class UpdateApplicationDtoValidator : AbstractValidator<UpdateApplicationDto>
{
    public UpdateApplicationDtoValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobPostingUrl).MaximumLength(2048);
        RuleFor(x => x.Salary).GreaterThanOrEqualTo(0).When(x => x.Salary.HasValue);
    }
}

public class UpdateStatusDtoValidator : AbstractValidator<UpdateStatusDto>
{
    public UpdateStatusDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
