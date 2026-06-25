using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
using JobLess.Client.Domain.Enums;

namespace JobLess.Tests.Client;

public class UpdateClientProfileCommandValidatorTests
{
    private readonly UpdateClientProfileCommandValidator _validator = new();

    private static UpdateClientProfileCommand ValidCommand() =>
        new(
            ClientId: 1,
            FirstName: "Marko",
            LastName: "Petrović",
            PhoneNumber: "+381601234567",
            Gender: Gender.Male,
            DateOfBirth: new DateTime(1990, 3, 10, 0, 0, 0, DateTimeKind.Utc),
            City: "Novi Sad",
            Address: null,
            EducationLevel: EducationLevel.Master,
            InstitutionName: "FTN",
            EducationStartYear: 2009,
            EducationEndYear: 2014,
            YearsOfExperience: 8,
            Skills: "C#, React",
            ProfessionalSummary: null,
            LinkedInUrl: null,
            IsActive: true);

    [Fact]
    public async Task Prihvata_ispravne_podatke()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Odbija_neispravan_id_klijenta(int clientId)
    {
        var command = ValidCommand() with { ClientId = clientId };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
    }

    [Theory]
    [InlineData("", "Petrović")]
    [InlineData("Marko", "")]
    public async Task Odbija_prazno_ime_ili_prezime(string firstName, string lastName)
    {
        var command = ValidCommand() with { FirstName = firstName, LastName = lastName };

        var result = await _validator.TestValidateAsync(command);

        if (string.IsNullOrEmpty(firstName))
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        if (string.IsNullOrEmpty(lastName))
            result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task Dozvoljava_profil_bez_telefona()
    {
        var command = ValidCommand() with { PhoneNumber = null };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public async Task Odbija_neispravan_pol()
    {
        var command = ValidCommand() with { Gender = (Gender)99 };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public async Task Odbija_godinu_zavrsetka_pre_godine_pocetka()
    {
        var command = ValidCommand() with { EducationStartYear = 2020, EducationEndYear = 2018 };

        var result = await _validator.TestValidateAsync(command);
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("završetka"));
    }
}
