using FluentValidation.TestHelper;
using JobLess.JobApplication.Application.Queries.GetMyApplications;

namespace JobLess.Tests.JobApplication;

public class GetMyApplicationsQueryValidatorTests
{
    private readonly GetMyApplicationsQueryValidator _validator = new();

    [Fact]
    public async Task Prihvata_ispravan_email()
    {
        var result = await _validator.TestValidateAsync(new GetMyApplicationsQuery("marko.petrovic@email.rs"));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("nije-validan-email")]
    public async Task Odbija_pogresan_email(string email)
    {
        var result = await _validator.TestValidateAsync(new GetMyApplicationsQuery(email));
        result.ShouldHaveValidationErrorFor(x => x.CandidateEmail);
    }
}
