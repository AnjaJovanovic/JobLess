using FluentAssertions;
using JobLess.Advertisement.Application.Commands.Update;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using MockQueryable.Moq;  
using Moq;


public class UpdateAdvertisementCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IValidationExceptionThrower> _validationMock;
    private readonly UpdateAdvertisementCommandHandler _handler;

    public UpdateAdvertisementCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _validationMock = new Mock<IValidationExceptionThrower>();
        _handler = new UpdateAdvertisementCommandHandler(_contextMock.Object, _validationMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Only_Specified_Fields_And_Keep_Others_Unchanged()
    {
        var existingAd = new JobAdvertisement
        {
            Id = 123,
            CompanyId = 1,
            Title = "Old Title",
            Description = "Old Description",
            Position = "Developer",
            EmploymentType = EmploymentType.Permanent,
            WorkSchedule = WorkSchedule.PartTime,
            SeniorityLevel = SeniorityLevel.Beginner,
            City = "Belgrade",
            Country = "Serbia",
            WorkType = WorkType.OnSite,
            SalaryFrom = 1500,
            SalaryTo = 2500,
            IsSalaryVisible = true,
            IsActive = true,
            Status = JobPostingStatus.Draft,
            PostedAt = System.DateTime.UtcNow
        };

        var mockDbSet = new List<JobAdvertisement> { existingAd }
            .AsQueryable()
            .BuildMockDbSet();

        _contextMock.Setup(c => c.JobAdvertisements).Returns(mockDbSet.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new UpdateAdvertisementCommand
        {
            Id = 123,
            Title = "Updated Title",
            Description = "Updated Description"
        };

     
        var result = await _handler.Handle(command, CancellationToken.None);

        existingAd.Title.Should().Be("Updated Title");
        existingAd.Description.Should().Be("Updated Description");
        existingAd.Id.Should().Be(123);
        existingAd.Position.Should().Be("Developer");
        existingAd.EmploymentType.Should().Be(EmploymentType.Permanent);
        existingAd.WorkSchedule.Should().Be(WorkSchedule.PartTime);
        existingAd.SeniorityLevel.Should().Be(SeniorityLevel.Beginner);
        existingAd.City.Should().Be("Belgrade");
        existingAd.Country.Should().Be("Serbia");
        existingAd.WorkType.Should().Be(WorkType.OnSite);
        existingAd.SalaryFrom.Should().Be(1500);
        existingAd.SalaryTo.Should().Be(2500);
        existingAd.IsSalaryVisible.Should().BeTrue();
        existingAd.IsActive.Should().BeTrue();
        existingAd.Status.Should().Be(JobPostingStatus.Draft);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
