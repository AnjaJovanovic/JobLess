using FluentAssertions;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Queries.GetByName;
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
    public class GetByNameCompanyQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly GetByNameCompanyQueryHandler _handler;

        public GetByNameCompanyQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new GetByNameCompanyQueryHandler(_contextMock.Object);
        }

        private CompanyEntity CreateCompany(int id, string name, bool isActive = true) => new CompanyEntity
        {
            Id = id,
            Name = name,
            Email = $"comp{id}@test.rs",
            TaxIdentificationNumber = $"{id:000000000}",
            RegistrationNumber = $"{id:00000000}",
            Industry = Industry.InformationTechnology,
            Location = "Beograd",
            ContactPersonFirstName = "Marko",
            ContactPersonLastName = "Markovic",
            ContactPersonPosition = "CEO",
            ContactPersonPhoneNumber = "0601234567",
            PasswordHash = "Sifra1234",
            CompanySize = CompanySize.OneToTen,
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
        public async Task Handle_Should_Return_Companies_Containing_Name()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1, "Infostud Solutions"),
                CreateCompany(2, "Infostud HR"),
                CreateCompany(3, "Nordeus d.o.o.")
            });

            var query = new GetByNameCompanyQuery { Name = "Infostud" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Companies.Should().HaveCount(2);
            result.Companies.Should().AllSatisfy(c => c.Name.Should().Contain("Infostud"));
        }

        [Fact]
        public async Task Handle_Should_Be_Case_Insensitive()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1, "DELTA Holdings"),
                CreateCompany(2, "Delta Holding Group")
            });

            var query = new GetByNameCompanyQuery { Name = "delta" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Match()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1, "Nordeus"),
                CreateCompany(2, "Nelt")
            });

            var query = new GetByNameCompanyQuery { Name = "Microsoft" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_Should_Exclude_Inactive_Companies()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1, "Infostud Active", isActive: true),
                CreateCompany(2, "Infostud Inactive", isActive: false)
            });

            var query = new GetByNameCompanyQuery { Name = "Infostud" };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(1);
            result.Companies![0].Name.Should().Be("Infostud Active");
        }
    }
}
