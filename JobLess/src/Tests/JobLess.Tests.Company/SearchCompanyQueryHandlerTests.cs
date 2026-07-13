using FluentAssertions;
using JobLess.Company.Application.Interfaces;
using JobLess.Company.Application.Queries.Search;
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
    public class SearchCompanyQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly SearchCompanyQueryHandler _handler;

        public SearchCompanyQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new SearchCompanyQueryHandler(_contextMock.Object);
        }

        private CompanyEntity CreateCompany(
            int id,
            string name,
            Industry industry,
            string location,
            bool isActive = true) => new CompanyEntity
            {
                Id = id,
                Name = name,
                Email = $"comp{id}@test.rs",
                TaxIdentificationNumber = $"{id:000000000}",
                RegistrationNumber = $"{id:00000000}",
                Industry = industry,
                Location = location,
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

        private List<CompanyEntity> SampleData() => new List<CompanyEntity>
        {
            CreateCompany(1, "TechCorp", Industry.InformationTechnology, "Beograd"),
            CreateCompany(2, "FinBank", Industry.FinanceAndBanking, "Novi Sad"),
            CreateCompany(3, "HealthPlus", Industry.Healthcare, "Beograd"),
            CreateCompany(4, "TechStart", Industry.InformationTechnology, "Nis"),
            CreateCompany(5, "InactiveFirm", Industry.Other, "Beograd", isActive: false)
        };

        [Fact]
        public async Task Handle_Should_Return_Only_Active_Companies_When_No_Filters()
        {
            // Arrange
            SetupDbSet(SampleData());

            var query = new SearchCompanyQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(4); // 5 ukupno, 1 neaktivan
            result.Companies.Should().AllSatisfy(c => c.IsActive.Should().BeTrue());
        }

        [Fact]
        public async Task Handle_Should_Filter_By_Name()
        {
            // Arrange
            SetupDbSet(SampleData());

            var query = new SearchCompanyQuery { Name = "Tech", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(2);
            result.Companies.Should().AllSatisfy(c => c.Name.Should().Contain("Tech"));
        }

        [Fact]
        public async Task Handle_Should_Filter_By_Industry()
        {
            // Arrange
            SetupDbSet(SampleData());

            var query = new SearchCompanyQuery { Industry = nameof(Industry.InformationTechnology), PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(2);
            result.Companies.Should().AllSatisfy(c => c.Industry.Should().Be(Industry.InformationTechnology));
        }

        [Fact]
        public async Task Handle_Should_Filter_By_Location()
        {
            // Arrange
            SetupDbSet(SampleData());

            var query = new SearchCompanyQuery { Location = "Beograd", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(2); // TechCorp i HealthPlus (InactiveFirm je neaktivan)
            result.Companies.Should().AllSatisfy(c => c.Location.Should().Be("Beograd"));
        }

        [Fact]
        public async Task Handle_Should_Filter_By_Multiple_Criteria()
        {
            // Arrange
            SetupDbSet(SampleData());

            var query = new SearchCompanyQuery
            {
                Industry = nameof(Industry.InformationTechnology),
                Location = "Beograd",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().HaveCount(1);
            result.Companies![0].Name.Should().Be("TechCorp");
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Pagination_Metadata()
        {
            // Arrange
            var companies = new List<CompanyEntity>();
            for (int i = 1; i <= 12; i++)
                companies.Add(CreateCompany(i, $"Kompanija{i}", Industry.Other, "Beograd"));

            SetupDbSet(companies);

            var query = new SearchCompanyQuery { PageNumber = 2, PageSize = 5 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(12);
            result.TotalPages.Should().Be(3); // ceil(12/5) = 3
            result.PageNumber.Should().Be(2);
            result.PageSize.Should().Be(5);
            result.Companies.Should().HaveCount(5);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_Result_When_No_Match()
        {
            // Arrange
            SetupDbSet(SampleData());

            var query = new SearchCompanyQuery { Name = "Nepostojeca", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Companies.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }
    }
}
