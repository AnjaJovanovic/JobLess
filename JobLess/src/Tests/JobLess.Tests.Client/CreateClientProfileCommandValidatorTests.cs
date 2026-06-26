using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
using JobLess.Client.Domain.Enums;

namespace JobLess.Tests.Client;

public class CreateClientProfileCommandValidatorTests
{
    private readonly CreateClientProfileCommandValidator _validator = new();

    private static CreateClientProfileCommand ValidCommand() =>
        new(
            Email: "marko.petrovic@email.rs",
            FirstName: "Marko",
            LastName: "Petrović",
            PhoneNumber: "+381601234567",
            Gender: Gender.Male,
            City: "Beograd",
            EducationLevel: EducationLevel.Bachelor,
            InstitutionName: "FON",
            EducationStartYear: 2014,
            EducationEndYear: 2018,
            YearsOfExperience: 2);

    [Fact]
    public async Task Prihvata_ispravne_podatke()
    {
        var result = await _validator.TestValidateAsync(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("nije-validan-email")]
    public async Task Odbija_pogresan_email(string email)
    {
        var command = ValidCommand() with { Email = email };

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
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
