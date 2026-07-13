using FluentAssertions;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Queries.GetAll;
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
    public class GetAllCompaniesQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly GetAllCompaniesQueryHandler _handler;

        public GetAllCompaniesQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new GetAllCompaniesQueryHandler(_contextMock.Object);
        }

        private CompanyEntity CreateCompany(int id, bool isActive) => new CompanyEntity
        {
            Id = id,
            Name = $"Kompanija {id}",
            Email = $"kompanija{id}@test.rs",
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
        public async Task Handle_Should_Return_Only_Active_Companies()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1, isActive: true),
                CreateCompany(2, isActive: true),
                CreateCompany(3, isActive: false)
            });

            var query = new GetAllCompaniesQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Companies.Should().HaveCount(2);
            result.Companies.Should().AllSatisfy(c => c.IsActive.Should().BeTrue());
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Pagination_Metadata()
        {
            // Arrange
            var companies = new List<CompanyEntity>();
            for (int i = 1; i <= 25; i++)
                companies.Add(CreateCompany(i, isActive: true));

            SetupDbSet(companies);

            var query = new GetAllCompaniesQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(25);
            result.TotalPages.Should().Be(3);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.Companies.Should().HaveCount(10);
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Page_When_PageNumber_Is_2()
        {
            // Arrange
            var companies = new List<CompanyEntity>();
            for (int i = 1; i <= 15; i++)
                companies.Add(CreateCompany(i, isActive: true));

            SetupDbSet(companies);

            var query = new GetAllCompaniesQuery { PageNumber = 2, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(5);
            result.PageNumber.Should().Be(2);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Active_Companies()
        {
            // Arrange
            SetupDbSet(new List<CompanyEntity>
            {
                CreateCompany(1, isActive: false),
                CreateCompany(2, isActive: false)
            });

            var query = new GetAllCompaniesQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
            result.TotalPages.Should().Be(0);
        }

        [Fact]
        public async Task Handle_Should_Calculate_TotalPages_Correctly_With_Remainder()
        {
            // Arrange
            var companies = new List<CompanyEntity>();
            for (int i = 1; i <= 11; i++)
                companies.Add(CreateCompany(i, isActive: true));

            SetupDbSet(companies);

            var query = new GetAllCompaniesQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalPages.Should().Be(2); // ceil(11/10) = 2
        }
    }
}
