using FluentValidation.TestHelper;
using JobLess.Client.Application.Clients.Commands.ApplyToJob;

namespace JobLess.Tests.Client;

public class ApplyToJobCommandValidatorTests
{
    private readonly ApplyToJobCommandValidator _validator = new();

    [Fact]
    public async Task Prihvata_ispravne_podatke()
    {
        var result = await _validator.TestValidateAsync(new ApplyToJobCommand(1, 42));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Odbija_neispravan_id_klijenta(int clientId)
    {
        var result = await _validator.TestValidateAsync(new ApplyToJobCommand(clientId, 42));
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Odbija_neispravan_id_oglasa(int advertisementId)
    {
        var result = await _validator.TestValidateAsync(new ApplyToJobCommand(1, advertisementId));
        result.ShouldHaveValidationErrorFor(x => x.AdvertisementId);
    }
}
