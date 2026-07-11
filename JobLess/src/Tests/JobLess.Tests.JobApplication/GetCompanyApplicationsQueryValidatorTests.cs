using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.JobApplication.Application.Queries.GetCompanyApplications;
using JobLess.JobApplication.Domain.Enums;

namespace JobLess.Tests.JobApplication;

public class GetCompanyApplicationsQueryValidatorTests
{
    private readonly GetCompanyApplicationsQueryValidator _validator = new();

    [Fact]
    public async Task Prihvata_ispravne_podatke()
    {
        var result = await _validator.TestValidateAsync(
            new GetCompanyApplicationsQuery("firma@kompanija.rs", AdvertisementId: 10, Status: JobApplicationStatus.Pending));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("nije-validan-email")]
    public async Task Odbija_pogresan_email(string email)
    {
        var result = await _validator.TestValidateAsync(new GetCompanyApplicationsQuery(email));
        result.ShouldHaveValidationErrorFor(x => x.CompanyEmail);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Odbija_nevalidan_advertisement_id(int advertisementId)
    {
        var result = await _validator.TestValidateAsync(
            new GetCompanyApplicationsQuery("firma@kompanija.rs", AdvertisementId: advertisementId));

        result.ShouldHaveValidationErrorFor(x => x.AdvertisementId);
    }

}
