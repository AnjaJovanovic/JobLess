using FluentAssertions;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Queries.GetOne;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CompanyEntity = JobLess.Company.Domain.Entities.Company;

namespace JobLess.Tests.Company
{
    public class GetOneCompanyQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly GetOneCompanyQueryHandler _handler;

        public GetOneCompanyQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new GetOneCompanyQueryHandler(_contextMock.Object);
        }

        private CompanyEntity CreateCompany(int id, bool isActive = true) => new CompanyEntity
        {
            Id = id,
            Name = "Test Kompanija",
            Description = "Opis kompanije",
            Email = "test@kompanija.rs",
            TaxIdentificationNumber = "123456789",
            RegistrationNumber = "12345678",
            Industry = "Informacione tehnologije",
            Location = "Beograd",
            ContactPersonFirstName = "Marko",
            ContactPersonLastName = "Markovic",
            ContactPersonPosition = "CEO",
            ContactPersonPhoneNumber = "0601234567",
            PasswordHash = "Sifra1234",
            CompanySize = "1-10",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        private void SetupDbSet(List<CompanyEntity> data)
        {
            var dbSetMock = data.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.Companies).Returns(dbSetMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Company_When_Exists_And_Active()
        {
            // Arrange
            var company = CreateCompany(1, isActive: true);
            SetupDbSet(new List<CompanyEntity> { company });

            var query = new GetOneCompanyQuery { Id = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Company.Should().NotBeNull();
            result.Company.Id.Should().Be(1);
            result.Company.Name.Should().Be("Test Kompanija");
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Company_Does_Not_Exist()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>());

            var query = new GetOneCompanyQuery { Id = 999 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Company_Is_Inactive()
        {
            // Arrange
            var company = CreateCompany(1, isActive: false);
            SetupDbSet(new List<CompanyEntity> { company });

            var query = new GetOneCompanyQuery { Id = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Company_When_Multiple_Exist()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1),
                CreateCompany(2),
                CreateCompany(3)
            });

            var query = new GetOneCompanyQuery { Id = 2 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Company.Id.Should().Be(2);
        }
    }
}
