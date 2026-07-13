using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;
using JobLess.JobApplication.Domain.Enums;

namespace JobLess.Tests.JobApplication;

public class UpdateApplicationStatusCommandValidatorTests
{
    private readonly UpdateApplicationStatusCommandValidator _validator = new();

    private static UpdateApplicationStatusCommand ValidCommand() =>
        new(ApplicationId: 1, CompanyEmail: "hr@tehnobit.rs", Status: JobApplicationStatus.Accepted);

    [Fact]
    public async Task Prihvata_accept_i_reject()
    {
        var accept = await _validator.TestValidateAsync(ValidCommand());
        accept.ShouldNotHaveAnyValidationErrors();

        var reject = await _validator.TestValidateAsync(
            ValidCommand() with { Status = JobApplicationStatus.Rejected });
        reject.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Odbija_pending_status()
    {
        var command = ValidCommand() with { Status = JobApplicationStatus.Pending };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public async Task Odbija_neispravan_email_kompanije()
    {
        var command = ValidCommand() with { CompanyEmail = "nije-email" };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.CompanyEmail);
    }
}
