using FluentAssertions;
using JobLess.Company.Application.Commands.Delete;
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
    public class DeleteCompanyCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly DeleteCompanyCommandHandler _handler;

        private const string CompanyEmailValue = "test@kompanija.rs";

        public DeleteCompanyCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new DeleteCompanyCommandHandler(_contextMock.Object);
        }

        private CompanyEntity CreateCompany(int id, bool isActive) => new CompanyEntity
        {
            Id = id,
            Name = "Test Kompanija",
            Email = CompanyEmailValue,
            TaxIdentificationNumber = "123456789",
            RegistrationNumber = "12345678",
            Industry = Industry.InformationTechnology,
            Location = "Beograd",
            ContactPersonFirstName = "Marko",
            ContactPersonLastName = "Markovic",
            ContactPersonPosition = "CEO",
            ContactPersonPhoneNumber = "0601234567",
            //PasswordHash = "Sifra1234",
            CompanySize = CompanySize.OneToTen,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
        public async Task Handle_Should_Deactivate_Company_When_Exists_And_Active()
        {
            // Arrange
            var company = CreateCompany(1, isActive: true);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new DeleteCompanyCommand { Id = 1, CompanyEmail = CompanyEmailValue };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            company.IsActive.Should().BeFalse();
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Update_UpdatedAt_On_Deactivation()
        {
            // Arrange
            var company = CreateCompany(1, isActive: true);
            var oldUpdatedAt = company.UpdatedAt;
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new DeleteCompanyCommand { Id = 1, CompanyEmail = CompanyEmailValue };

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

            var command = new DeleteCompanyCommand { Id = 999, CompanyEmail = CompanyEmailValue };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_False_When_Company_Already_Inactive()
        {
            // Arrange
            var company = CreateCompany(1, isActive: false);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new DeleteCompanyCommand { Id = 1, CompanyEmail = CompanyEmailValue };

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
            var company = CreateCompany(1, isActive: true);
            SetupDbSet(new List<CompanyEntity> { company });

            var command = new DeleteCompanyCommand { Id = 1, CompanyEmail = "pogresan@email.rs" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            company.IsActive.Should().BeTrue();
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
