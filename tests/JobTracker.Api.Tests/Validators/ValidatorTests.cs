using FluentValidation.TestHelper;
using JobTracker.Api.DTOs;
using JobTracker.Api.Models;
using JobTracker.Api.Validators;

namespace JobTracker.Api.Tests.Validators;

public class ValidatorTests
{
    [Fact]
    public void CreateApplication_Valid_HasNoErrors()
    {
        var dto = new CreateApplicationDto("Acme", "Engineer", null, ApplicationSource.Other, 1000m, null, WorkType.Remote, null, null, null);

        new CreateApplicationDtoValidator().TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateApplication_Invalid_ReportsEachRule()
    {
        var dto = new CreateApplicationDto("", new string('x', 201), new string('u', 2049), ApplicationSource.Other, -1m, null, WorkType.Remote, null, null, null);

        var result = new CreateApplicationDtoValidator().TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
        result.ShouldHaveValidationErrorFor(x => x.JobTitle);
        result.ShouldHaveValidationErrorFor(x => x.JobPostingUrl);
        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void UpdateApplication_RequiresCompanyAndTitle()
    {
        var dto = new UpdateApplicationDto("", "", null, ApplicationSource.Other, null, null, WorkType.Remote, DateTime.UtcNow, null, null);

        var result = new UpdateApplicationDtoValidator().TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
        result.ShouldHaveValidationErrorFor(x => x.JobTitle);
        result.ShouldNotHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void UpdateStatus_RejectsUndefinedEnumValue()
    {
        var validator = new UpdateStatusDtoValidator();

        validator.TestValidate(new UpdateStatusDto((ApplicationStatus)99, null)).ShouldHaveValidationErrorFor(x => x.Status);
        validator.TestValidate(new UpdateStatusDto(ApplicationStatus.Offer, null)).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("not-an-email", "password123")]
    [InlineData("", "password123")]
    [InlineData("jane@acme.test", "short")]
    public void Register_InvalidInput_HasErrors(string email, string password)
    {
        new RegisterDtoValidator().TestValidate(new RegisterDto(email, password, null)).ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Register_Valid_HasNoErrors()
    {
        new RegisterDtoValidator().TestValidate(new RegisterDto("jane@acme.test", "password123", null)).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Login_RequiresPassword()
    {
        new LoginDtoValidator().TestValidate(new LoginDto("jane@acme.test", "")).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("jane@acme.test")]
    public void CreateContact_EmailIsOptionalButMustBeValidWhenPresent(string? email)
    {
        new CreateContactDtoValidator().TestValidate(new CreateContactDto("Jane", null, email, null, null)).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateContact_Invalid_HasErrors()
    {
        var result = new CreateContactDtoValidator().TestValidate(new CreateContactDto("", null, "bad-email", null, null));

        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateReminder_RequiresMessageAndDueDate()
    {
        var result = new CreateReminderDtoValidator().TestValidate(new CreateReminderDto(default, ""));

        result.ShouldHaveValidationErrorFor(x => x.Message);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void UpdateReminder_RejectsTooLongMessage()
    {
        var dto = new UpdateReminderDto(DateTime.UtcNow, new string('m', 501), false);

        new UpdateReminderDtoValidator().TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Message);
    }
}
