using FluentAssertions;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Queries.Search;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobLess.Tests.Advertisement
{
    public class SearchAdvertisementQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly SearchAdvertisementQueryHandler _handler;

        public SearchAdvertisementQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new SearchAdvertisementQueryHandler(_contextMock.Object);
        }

        private void SetupDbSet(List<JobAdvertisement> data)
        {
            var dbSetMock = data.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.JobAdvertisements).Returns(dbSetMock.Object);
        }

        private JobAdvertisement CreateAd(int id, string title = "Backend Developer",
            string city = "Belgrade", EmploymentType employmentType = EmploymentType.Internship,
            WorkType workType = WorkType.Hybrid, decimal salaryFrom = 1000, decimal salaryTo = 2000)
            => new JobAdvertisement
            {
                Id = id,
                CompanyId = 1,
                Title = title,
                Description = "Test",
                Position = "Developer",
                EmploymentType = employmentType,
                WorkSchedule = WorkSchedule.PartTime,
                SeniorityLevel = SeniorityLevel.Beginner,
                City = city,
                Country = "Serbia",
                WorkType = workType,
                SalaryFrom = salaryFrom,
                SalaryTo = salaryTo,
                IsSalaryVisible = true,
                Status = JobPostingStatus.Draft,
                IsActive = true,
                PostedAt = System.DateTime.UtcNow
            };

        [Fact]
        public async Task Handle_Should_Return_All_Active_Ads_When_No_Filters()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1),
                CreateAd(2),
                CreateAd(3)
            });

            var query = new SearchAdvertisementQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(3);
            result.Advertisements.Should().HaveCount(3);
        }

        [Fact]
        public async Task Handle_Should_Filter_By_Title()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, title: "Backend Developer"),
                CreateAd(2, title: "Frontend Developer"),
                CreateAd(3, title: "Designer")
            });

            var query = new SearchAdvertisementQuery { Title = "Developer", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(2);
            result.Advertisements.Should().AllSatisfy(a => a.Title.Should().Contain("Developer"));
        }

        [Fact]
        public async Task Handle_Should_Filter_By_City()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, city: "Belgrade"),
                CreateAd(2, city: "Belgrade"),
                CreateAd(3, city: "Novi Sad")
            });

            var query = new SearchAdvertisementQuery { City = "Belgrade", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(2);
            result.Advertisements.Should().AllSatisfy(a => a.City.Should().Be("Belgrade"));
        }

        [Fact]
        public async Task Handle_Should_Filter_By_EmploymentType()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, employmentType: EmploymentType.Internship),
                CreateAd(2, employmentType: EmploymentType.Permanent),
                CreateAd(3, employmentType: EmploymentType.Internship)
            });

            var query = new SearchAdvertisementQuery { EmploymentType = EmploymentType.Internship, PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(2);
            result.Advertisements.Should().AllSatisfy(a => a.EmploymentType.Should().Be(EmploymentType.Internship));
        }

        [Fact]
        public async Task Handle_Should_Filter_By_SalaryRange()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, salaryFrom: 1000, salaryTo: 2000),
                CreateAd(2, salaryFrom: 3000, salaryTo: 5000),
                CreateAd(3, salaryFrom: 500,  salaryTo: 800)
            });

            var query = new SearchAdvertisementQuery { SalaryFrom = 1000, SalaryTo = 2000, PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.TotalCount.Should().Be(1);
            result.Advertisements[0].SalaryFrom.Should().BeGreaterThanOrEqualTo(1000);
            result.Advertisements[0].SalaryTo.Should().BeLessThanOrEqualTo(2000);
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Pagination()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1), CreateAd(2), CreateAd(3), CreateAd(4), CreateAd(5)
            });

            var query = new SearchAdvertisementQuery { PageNumber = 2, PageSize = 2 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().HaveCount(2);
            result.TotalCount.Should().Be(5);
            result.TotalPages.Should().Be(3);
            result.PageNumber.Should().Be(2);
        }
    }
}