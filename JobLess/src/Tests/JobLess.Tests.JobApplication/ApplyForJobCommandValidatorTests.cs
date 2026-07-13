using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.JobApplication.Application.Commands.ApplyForJob;

namespace JobLess.Tests.JobApplication;

public class ApplyForJobCommandValidatorTests
{
    private readonly ApplyForJobCommandValidator _validator = new();

    private static ApplyForJobCommand ValidCommand() =>
        new(AdvertisementId: 10, CompanyId: 2, CandidateEmail: "marko.petrovic@email.rs");

    [Fact]
    public async Task Prihvata_ispravne_podatke()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Odbija_neispravan_advertisement_id(int advertisementId)
    {
        var command = ValidCommand() with { AdvertisementId = advertisementId };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.AdvertisementId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("nije-email")]
    public async Task Odbija_pogresan_email(string email)
    {
        var command = ValidCommand() with { CandidateEmail = email };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.CandidateEmail);
    }
}
