using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;
using JobLess.JobApplication.Domain.Enums;

namespace JobLess.Tests.JobApplication;

public class UpdateApplicationStatusCommandValidatorTests
{
    private readonly UpdateApplicationStatusCommandValidator _validator = new();

    private static UpdateApplicationStatusCommand ValidCommand() =>
        new(ApplicationId: 1, CompanyEmail: "firma@kompanija.rs", Status: JobApplicationStatus.Accepted);

    [Fact]
    public async Task Prihvata_ispravne_podatke()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Odbija_nevalidan_application_id(int applicationId)
    {
        var command = ValidCommand() with { ApplicationId = applicationId };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ApplicationId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("nije-validan-email")]
    public async Task Odbija_pogresan_email(string email)
    {
        var command = ValidCommand() with { CompanyEmail = email };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.CompanyEmail);
    }

    [Fact]
    public async Task Odbija_pending_status()
    {
        var command = ValidCommand() with { Status = JobApplicationStatus.Pending };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
