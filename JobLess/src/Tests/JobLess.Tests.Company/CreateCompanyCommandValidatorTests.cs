using FluentAssertions;
using FluentValidation.TestHelper;
using JobLess.Company.Application.Commands.Create;
using JobLess.Company.Domain.Enums;
using System.Threading.Tasks;

namespace JobLess.Tests.Company
{
    public class CreateCompanyCommandValidatorTests
    {
        private readonly CreateCompanyCommandValidator _validator;

        public CreateCompanyCommandValidatorTests()
        {
            _validator = new CreateCompanyCommandValidator();
        }

        private CreateCompanyCommand ValidCommand() => new CreateCompanyCommand
        {
            Name = "Test Kompanija",
            Industry = "Informacione tehnologije",
            Location = "Beograd",
            OwnerId = 1,
            TaxIdentificationNumber = "123456789",
            RegistrationNumber = "12345678",
            ContactPersonFirstName = "Marko",
            ContactPersonLastName = "Markovic",
            ContactPersonPosition = "CEO",
            ContactPersonPhoneNumber = "+381 60 123 4567",
            Email = "test@kompanija.rs",
            //PasswordHash = "Sifra1234",
            CompanySize = CompanySize.OneToTen
        };

        [Fact]
        public async Task Should_Pass_Validation_For_Valid_Command()
        {
            var result = await _validator.TestValidateAsync(ValidCommand());
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task Should_Fail_When_Name_Is_Empty()
        {
            var command = ValidCommand();
            command.Name = "";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Should_Fail_When_Name_Exceeds_200_Characters()
        {
            var command = ValidCommand();
            command.Name = new string('A', 201);

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Should_Fail_When_Industry_Is_Invalid()
        {
            var command = ValidCommand();
            command.Industry = "Neka Nepostojeca Delatnost";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Industry);
        }

        [Theory]
        [InlineData("Informacione tehnologije")]
        [InlineData("Finansije i bankarstvo")]
        [InlineData("Zdravstvo")]
        [InlineData("Ostalo")]
        public async Task Should_Pass_For_Valid_Industry(string industry)
        {
            var command = ValidCommand();
            command.Industry = industry;

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Industry);
        }

        [Fact]
        public async Task Should_Fail_When_CompanySize_Is_Invalid()
        {
            var command = ValidCommand();
            command.CompanySize = (CompanySize)999;

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.CompanySize);
        }

        [Theory]
        [InlineData(CompanySize.OneToTen)]
        [InlineData(CompanySize.ElevenToFifty)]
        [InlineData(CompanySize.FiftyOneToTwoHundred)]
        [InlineData(CompanySize.TwoHundredOneToFiveHundred)]
        [InlineData(CompanySize.MoreThanFiveHundred)]
        public async Task Should_Pass_For_Valid_CompanySize(CompanySize size)
        {
            var command = ValidCommand();
            command.CompanySize = size;

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.CompanySize);
        }

        [Fact]
        public async Task Should_Fail_When_TaxIdentificationNumber_Is_Not_9_Digits()
        {
            var command = ValidCommand();
            command.TaxIdentificationNumber = "12345"; // premalo cifara

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.TaxIdentificationNumber);
        }

        [Fact]
        public async Task Should_Fail_When_TaxIdentificationNumber_Contains_Letters()
        {
            var command = ValidCommand();
            command.TaxIdentificationNumber = "12345678A";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.TaxIdentificationNumber);
        }

        [Fact]
        public async Task Should_Fail_When_RegistrationNumber_Is_Not_8_Digits()
        {
            var command = ValidCommand();
            command.RegistrationNumber = "1234567"; // 7 umesto 8

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.RegistrationNumber);
        }

        [Fact]
        public async Task Should_Fail_When_Email_Is_Invalid()
        {
            var command = ValidCommand();
            command.Email = "nije-email";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public async Task Should_Not_Fail_When_OwnerId_Is_Zero()
        {
            // OwnerId pravilo je trenutno zakomentarisano u CreateCompanyCommandValidator,
            // pa validator ne baca grešku čak ni kad je OwnerId 0.
            var command = ValidCommand();
            command.OwnerId = 0;

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.OwnerId);
        }

        [Fact]
        public async Task Should_Pass_When_Website_Is_Valid_Url()
        {
            var command = ValidCommand();
            command.Website = "www.kompanija.rs";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Website);
        }

        [Fact]
        public async Task Should_Pass_When_Website_Is_Null()
        {
            var command = ValidCommand();
            command.Website = null;

            var result = await _validator.TestValidateAsync(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Website);
        }

        [Fact]
        public async Task Should_Fail_When_Description_Exceeds_2000_Characters()
        {
            var command = ValidCommand();
            command.Description = new string('X', 2001);

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public async Task Should_Fail_When_ContactPersonFirstName_Is_Empty()
        {
            var command = ValidCommand();
            command.ContactPersonFirstName = "";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.ContactPersonFirstName);
        }

        [Fact]
        public async Task Should_Fail_When_Location_Is_Empty()
        {
            var command = ValidCommand();
            command.Location = "";

            var result = await _validator.TestValidateAsync(command);
            result.ShouldHaveValidationErrorFor(x => x.Location);
        }
    }
}
