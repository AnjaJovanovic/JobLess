using FluentAssertions;
using JobLess.Company.Application.Commands.Update;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CompanyEntity = JobLess.Company.Domain.Entities.Company;

namespace JobLess.Tests.Company
{
    public class UpdateCompanyCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly UpdateCompanyCommandHandler _handler;

        private const string CompanyEmailValue = "stari@email.rs";

        public UpdateCompanyCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new UpdateCompanyCommandHandler(_contextMock.Object);
        }

        private CompanyEntity CreateCompany(int id, bool isActive = true) => new CompanyEntity
        {
            Id = id,
            Name = "Staro Ime",
            Description = "Stari opis",
            Industry = Industry.Healthcare,
            Location = "Novi Sad",
            Website = "www.stara.rs",
            Email = CompanyEmailValue,
            TaxIdentificationNumber = "111111111",
            RegistrationNumber = "11111111",
            OwnerName = "Stari Vlasnik",
            ContactPersonFirstName = "Ana",
            ContactPersonLastName = "Anic",
            ContactPersonPosition = "HR",
            ContactPersonPhoneNumber = "0601111111",
            //PasswordHash = "StaraSifra1",
            PhoneNumber = "0111111111",
            Address = "Stara ulica 1",
            CompanySize = CompanySize.OneToTen,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        private void SetupDbSet(List<CompanyEntity> data)
        {
            var dbSetMock = data.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Companies).Returns(dbSetMock.Object);
            _contextMock
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
        }

        [Fact]
        public async Task Handle_Should_Update_Name_When_Valid_And_Different()
        {
            // Arrange
            var company = CreateCompany(1);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Name = "Novo Ime Kompanije"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            company.Name.Should().Be("Novo Ime Kompanije");
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Not_Update_Field_When_Value_Is_String_Literal()
        {
            // Arrange
            var company = CreateCompany(1);
            var originalName = company.Name;
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Name = "string" // treba da se ignorise
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            company.Name.Should().Be(originalName);
        }

        [Fact]
        public async Task Handle_Should_Not_Update_Field_When_Value_Is_Null_Or_Whitespace()
        {
            // Arrange
            var company = CreateCompany(1);
            var originalDesc = company.Description;
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Description = null
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            company.Description.Should().Be(originalDesc);
        }

        [Fact]
        public async Task Handle_Should_Update_Multiple_Fields()
        {
            // Arrange
            var company = CreateCompany(1);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Name = "Novo Ime",
                Location = "Beograd",
                ContactPersonPhoneNumber = "0609999999",
                Address = "Nova ulica 5",
                EmployeeCount = 150
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            company.Name.Should().Be("Novo Ime");
            company.Location.Should().Be("Beograd");
            company.ContactPersonPhoneNumber.Should().Be("0609999999");
            company.Address.Should().Be("Nova ulica 5");
            company.CompanySize.Should().Be(CompanySize.FiftyOneToTwoHundred);
        }

        [Fact]
        public async Task Handle_Should_Update_UpdatedAt()
        {
            // Arrange
            var company = CreateCompany(1);
            var oldUpdatedAt = company.UpdatedAt;
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Name = "Novo Ime"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            company.UpdatedAt.Should().BeOnOrAfter(oldUpdatedAt);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_Company_Does_Not_Exist()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>());

            var command = new UpdateCompanyCommand
            {
                CompanyId = 999,
                CompanyEmail = CompanyEmailValue,
                Name = "Neko Ime"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_Company_Is_Inactive()
        {
            // Arrange
            var company = CreateCompany(1, isActive: false);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Name = "Neko Ime"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_CompanyEmail_Does_Not_Match()
        {
            // Arrange
            var company = CreateCompany(1);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = "pogresan@email.rs",
                Name = "Neko Ime"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            company.Name.Should().Be("Staro Ime");
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Not_Update_Field_When_Value_Is_Same_As_Existing()
        {
            // Arrange
            var company = CreateCompany(1);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new UpdateCompanyCommand
            {
                CompanyId = 1,
                CompanyEmail = CompanyEmailValue,
                Name = company.Name // isti naziv
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            company.Name.Should().Be("Staro Ime");
            // SaveChanges se i dalje poziva (UpdatedAt se menja)
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
