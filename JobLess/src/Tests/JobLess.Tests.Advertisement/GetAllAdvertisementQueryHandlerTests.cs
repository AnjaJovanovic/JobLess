using FluentAssertions;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Queries.GetAll;
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
    public class GetAllAdvertisementQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly GetAllAdvertisementQueryHandler _handler;

        public GetAllAdvertisementQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _handler = new GetAllAdvertisementQueryHandler(_contextMock.Object);
        }

        private void SetupDbSet(List<JobAdvertisement> data)
        {
            var dbSetMock = data.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.JobAdvertisements).Returns(dbSetMock.Object);
        }

        private JobAdvertisement CreateAd(int id, bool isActive) => new JobAdvertisement
        {
            Id = id,
            CompanyId = 1,
            Title = "Backend Developer",
            Description = "Test",
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
            Status = JobPostingStatus.Draft,
            IsActive = isActive,
            PostedAt = System.DateTime.UtcNow
        };

        [Fact]
        public async Task Handle_Should_Return_Only_Active_Advertisements()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, isActive: true),
                CreateAd(2, isActive: true),
                CreateAd(3, isActive: false)
            });

            var query = new GetAllAdvertisementQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Advertisements.Should().HaveCount(2);
            result.Advertisements.Should().AllSatisfy(a => a.IsActive.Should().BeTrue());
        }

        [Fact]
        public async Task Handle_Should_Return_Correct_Pagination()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, isActive: true),
                CreateAd(2, isActive: true),
                CreateAd(3, isActive: true),
                CreateAd(4, isActive: true),
                CreateAd(5, isActive: true)
            });

            var query = new GetAllAdvertisementQuery { PageNumber = 1, PageSize = 3 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().HaveCount(3);
            result.TotalCount.Should().Be(5);
            result.TotalPages.Should().Be(2);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(3);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_When_No_Active_Advertisements()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, isActive: false),
                CreateAd(2, isActive: false)
            });

            var query = new GetAllAdvertisementQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Advertisements.Should().BeEmpty();
        }
    }
}