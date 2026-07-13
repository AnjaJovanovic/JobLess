using FluentAssertions;
using JobLess.Company.Application.Commands.Create;
using JobLess.Company.Application.Common.Helpers;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Domain.Entities;
using JobLess.Company.Domain.Enums;
using CompanyEntity = JobLess.Company.Domain.Entities.Company;
using JobLess.Shared.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobLess.Tests.Company
{
    public class CreateCompanyCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IValidationExceptionThrower> _validationThrowerMock;
        private readonly CreateCompanyCommandHandler _handler;

        public CreateCompanyCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _validationThrowerMock = new Mock<IValidationExceptionThrower>();
            _handler = new CreateCompanyCommandHandler(_contextMock.Object, _validationThrowerMock.Object);
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
            ContactPersonPhoneNumber = "0601234567",
            Email = "test@kompanija.rs",
            //PasswordHash = "Sifra1234",
            CompanySize = CompanySize.OneToTen
        };

        private void SetupEmptyCompanyDbSet()
        {
            var emptyList = new List<CompanyEntity>();
            var dbSetMock = emptyList.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Companies).Returns(dbSetMock.Object);

            var adminDbSetMock = new Mock<DbSet<CompanyAdmin>>();
            _contextMock.Setup(c => c.CompanyAdmins).Returns(adminDbSetMock.Object);

            _contextMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
        }

        private void SetupCompanyDbSetWithExisting(List<CompanyEntity> companies)
        {
            var dbSetMock = companies.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Companies).Returns(dbSetMock.Object);
        }

        private void SetupThrowerToThrow(string expectedMessage)
        {
            _validationThrowerMock
                .Setup(v => v.ThrowValidationException("Id", expectedMessage))
                .Throws(new Exception(expectedMessage));
        }

        [Fact]
        public async Task Handle_Should_Create_Company_And_Return_Id()
        {
            // Arrange
            SetupEmptyCompanyDbSet();
            var command = ValidCommand();

            CompanyEntity? capturedCompany = null;
            _contextMock.Setup(c => c.Companies.Add(It.IsAny<CompanyEntity>()))
                .Callback<CompanyEntity>(c =>
                {
                    c.Id = 42;
                    capturedCompany = c;
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(42);
            capturedCompany.Should().NotBeNull();
            capturedCompany!.Name.Should().Be(command.Name);
            capturedCompany.Email.Should().Be(command.Email);
            capturedCompany.Industry.Should().Be(IndustryHelper.GetIndustry(command.Industry));
            capturedCompany.IsActive.Should().BeTrue();
            capturedCompany.TaxIdentificationNumber.Should().Be(command.TaxIdentificationNumber);
            capturedCompany.RegistrationNumber.Should().Be(command.RegistrationNumber);
        }

        [Fact]
        public async Task Handle_Should_Set_IsActive_True_And_Timestamps_On_Create()
        {
            // Arrange
            SetupEmptyCompanyDbSet();
            var command = ValidCommand();
            var beforeCreate = DateTime.UtcNow;

            CompanyEntity? capturedCompany = null;
            _contextMock.Setup(c => c.Companies.Add(It.IsAny<CompanyEntity>()))
                .Callback<CompanyEntity>(c => capturedCompany = c);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedCompany!.IsActive.Should().BeTrue();
            capturedCompany.CreatedAt.Should().BeOnOrAfter(beforeCreate);
            capturedCompany.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Company_With_Same_Email_Exists()
        {
            // Arrange
            var command = ValidCommand();
            var existingCompanies = new List<CompanyEntity>
            {
                new CompanyEntity
                {
                    Id = 1,
                    Name = "Postojeca",
                    Email = command.Email, // isti email
                    TaxIdentificationNumber = "999999999",
                    RegistrationNumber = "99999999",
                    Industry = Industry.Other,
                    Location = "Novi Sad",
                    ContactPersonFirstName = "Ana",
                    ContactPersonLastName = "Anic",
                    ContactPersonPosition = "HR",
                    ContactPersonPhoneNumber = "0611111111",
                   // PasswordHash = "Hash1234",
                    CompanySize = CompanySize.OneToTen,
                    IsActive = true
                }
            };
            SetupCompanyDbSetWithExisting(existingCompanies);

            var expectedMessage = "Kompanija sa unetim PIB-om, matičnim brojem ili email adresom već postoji.";
            SetupThrowerToThrow(expectedMessage);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*već postoji*");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Company_With_Same_TaxId_Exists()
        {
            // Arrange
            var command = ValidCommand();
            var existingCompanies = new List<CompanyEntity>
            {
                new CompanyEntity
                {
                    Id = 1,
                    Name = "Postojeca",
                    Email = "drugi@email.rs",
                    TaxIdentificationNumber = command.TaxIdentificationNumber, // isti PIB
                    RegistrationNumber = "99999999",
                    Industry = Industry.Other,
                    Location = "Novi Sad",
                    ContactPersonFirstName = "Ana",
                    ContactPersonLastName = "Anic",
                    ContactPersonPosition = "HR",
                    ContactPersonPhoneNumber = "0611111111",
                  //  PasswordHash = "Hash1234",
                    CompanySize = CompanySize.OneToTen,
                    IsActive = true
                }
            };
            SetupCompanyDbSetWithExisting(existingCompanies);

            var expectedMessage = "Kompanija sa unetim PIB-om, matičnim brojem ili email adresom već postoji.";
            SetupThrowerToThrow(expectedMessage);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*već postoji*");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Company_With_Same_RegistrationNumber_Exists()
        {
            // Arrange
            var command = ValidCommand();
            var existingCompanies = new List<CompanyEntity>
            {
                new CompanyEntity
                {
                    Id = 1,
                    Name = "Postojeca",
                    Email = "drugi@email.rs",
                    TaxIdentificationNumber = "999999999",
                    RegistrationNumber = command.RegistrationNumber, // isti maticni broj
                    Industry = Industry.Other,
                    Location = "Novi Sad",
                    ContactPersonFirstName = "Ana",
                    ContactPersonLastName = "Anic",
                    ContactPersonPosition = "HR",
                    ContactPersonPhoneNumber = "0611111111",
                 //   PasswordHash = "Hash1234",
                    CompanySize = CompanySize.OneToTen,
                    IsActive = true
                }
            };
            SetupCompanyDbSetWithExisting(existingCompanies);

            var expectedMessage = "Kompanija sa unetim PIB-om, matičnim brojem ili email adresom već postoji.";
            SetupThrowerToThrow(expectedMessage);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*već postoji*");
        }

        [Fact]
        public async Task Handle_Should_Add_CompanyAdmin_With_Owner_Role()
        {
            // Arrange
            SetupEmptyCompanyDbSet();
            var command = ValidCommand();

            CompanyAdmin? capturedAdmin = null;
            _contextMock.Setup(c => c.CompanyAdmins.Add(It.IsAny<CompanyAdmin>()))
                .Callback<CompanyAdmin>(a => capturedAdmin = a);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            capturedAdmin.Should().NotBeNull();
            capturedAdmin!.Role.Should().Be("Owner");
            capturedAdmin.UserId.Should().Be(command.OwnerId);
        }

        [Fact]
        public async Task Handle_Should_Call_SaveChangesAsync_Twice()
        {
            // Arrange
            SetupEmptyCompanyDbSet();
            var command = ValidCommand();

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
