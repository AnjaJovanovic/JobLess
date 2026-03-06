using FluentAssertions;
using Jobless.Advertisement.Application.Queries.GetAllForCompany;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Queries.GetAllForCompany;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobLess.Tests.Advertisement
{
    public class GetAllForCompanyQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly GetAllForComapnyQueryHandler _handler;

        public GetAllForCompanyQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new GetAllForComapnyQueryHandler(_contextMock.Object);
        }

        private void SetupDbSet(List<JobAdvertisement> data)
        {
            var dbSetMock = data.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.JobAdvertisements).Returns(dbSetMock.Object);
        }

        private JobAdvertisement CreateAd(int id, int companyId, bool isActive, DateTime? expiresAt = null) => new JobAdvertisement
        {
            Id = id,
            CompanyId = companyId,
            Title = "Backend Developer",
            Description = "Test opis",
            Position = "Developer",
            EmploymentType = EmploymentType.Internship,
            WorkSchedule = WorkSchedule.PartTime,
            SeniorityLevel = SeniorityLevel.Beginner,
            City = "Belgrade",
            Country = "Serbia",
            WorkType = WorkType.Hybrid,
            SalaryFrom = 1000,
            SalaryTo = 2000,
            IsSalaryVisible = true,
            IsActive = isActive,
            ExpiresAt = expiresAt,
            PostedAt = DateTime.UtcNow
        };

        [Fact]
        public async Task Handle_Should_Return_All_Ads_For_Given_Company()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, companyId: 1, isActive: true),
                CreateAd(2, companyId: 1, isActive: true),
                CreateAd(3, companyId: 1, isActive: false),
                CreateAd(4, companyId: 2, isActive: true)
            });

            var query = new GetAllForCompanyQuery
            {
                CompanyId = 1,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Advertisements.Should().HaveCount(3);
            result.Advertisements.Should().AllSatisfy(a =>
            {
                a.CompanyId.Should().Be(1);
            });
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_When_Company_Has_No_Ads()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, companyId: 2, isActive: true),
                CreateAd(2, companyId: 2, isActive: false)
            });

            var query = new GetAllForCompanyQuery
            {
                CompanyId = 1,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Pagination_For_Company()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, companyId: 1, isActive: true),
                CreateAd(2, companyId: 1, isActive: false),
                CreateAd(3, companyId: 1, isActive: true),
                CreateAd(4, companyId: 1, isActive: false),
            });

            var query = new GetAllForCompanyQuery
            {
                CompanyId = 1,
                PageNumber = 1,
                PageSize = 2
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().HaveCount(2);
            result.TotalCount.Should().Be(4);
            result.TotalPages.Should().Be(2);
        }

        [Fact]
        public async Task Handle_Should_Not_Return_Ads_From_Other_Companies()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
               CreateAd(1, companyId: 1, isActive: true),
               CreateAd(2, companyId: 1, isActive: false),
               CreateAd(3, companyId: 2, isActive: true),
               CreateAd(4, companyId: 3, isActive: true)
            });

            var query = new GetAllForCompanyQuery
            {
                CompanyId = 1,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().HaveCount(2);
            result.Advertisements.Should().OnlyContain(a => a.CompanyId == 1);
        }

        [Fact]
        public async Task Handle_Should_Not_Return_Expired_Ads_For_Company()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, companyId: 1, isActive: true,  expiresAt: DateTime.UtcNow.AddDays(10)),  // validan
                CreateAd(2, companyId: 1, isActive: false, expiresAt: DateTime.UtcNow.AddDays(5)),   // validan, neaktivan
                CreateAd(3, companyId: 1, isActive: true,  expiresAt: DateTime.UtcNow.AddDays(-1)),  // istekao
                CreateAd(4, companyId: 1, isActive: true,  expiresAt: null),                         // validan, bez datuma
                CreateAd(5, companyId: 2, isActive: true,  expiresAt: DateTime.UtcNow.AddDays(10))   // druga kompanija
            });

            var query = new GetAllForCompanyQuery
            {
                CompanyId = 1,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().HaveCount(3);
            result.Advertisements.Should().OnlyContain(a => a.CompanyId == 1);
            result.Advertisements.Should().NotContain(a => a.Id == 3);
        }
    }
}