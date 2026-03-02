using FluentAssertions;
using JobLess.Advertisement.Application.Commands.Update;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
/*
public class UpdateAdvertisementCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<DbSet<JobAdvertisement>> _dbSetMock;
    private readonly Mock<IValidationExceptionThrower> _validationMock;
    private readonly UpdateAdvertisementCommandHandler _handler;

    public UpdateAdvertisementCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _dbSetMock = new Mock<DbSet<JobAdvertisement>>();
        _validationMock = new Mock<IValidationExceptionThrower>();

        _contextMock.Setup(c => c.JobAdvertisements).Returns(_dbSetMock.Object);

        _handler = new UpdateAdvertisementCommandHandler(_contextMock.Object, _validationMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Only_Specified_Fields_And_Keep_Others_Unchanged()
    {
        // Arrange - postojeći oglas
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

        // Napravi IQueryable listu za LINQ
        var ads = new List<JobAdvertisement> { existingAd }.AsQueryable();

        _dbSetMock.As<IQueryable<JobAdvertisement>>().Setup(m => m.Provider).Returns(ads.Provider);
        _dbSetMock.As<IQueryable<JobAdvertisement>>().Setup(m => m.Expression).Returns(ads.Expression);
        _dbSetMock.As<IQueryable<JobAdvertisement>>().Setup(m => m.ElementType).Returns(ads.ElementType);
        _dbSetMock.As<IQueryable<JobAdvertisement>>().Setup(m => m.GetEnumerator()).Returns(ads.GetEnumerator());

        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new UpdateAdvertisementCommand
        {
            Id = 123,
            Title = "Updated Title",
            Description = "Updated Description"
            // ostala polja nisu navedena i neće se menjati
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        //result.Should().Be(123);

        existingAd.Title.Should().Be("Updated Title");
        existingAd.Description.Should().Be("Updated Description");

        // ostala polja nepromenjena
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
*/