using FluentAssertions;
using JobLess.Advertisement.Application.Commands.Create;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

public class CreateAdvertisementCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<DbSet<JobAdvertisement>> _dbSetMock;
    private readonly CreateAdvertisementCommandHandler _handler;

    public CreateAdvertisementCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _dbSetMock = new Mock<DbSet<JobAdvertisement>>();

        _contextMock
            .Setup(c => c.JobAdvertisements)
            .Returns(_dbSetMock.Object);

        _handler = new CreateAdvertisementCommandHandler(_contextMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Advertisement_And_Save_To_Database()
    {

        var command = new CreateAdvertisementCommand
        {
            CompanyId = 1,
            Title = "Backend Developer",
            Description = "Test opis",
            Position = "Developer",
            EmploymentType = EmploymentType.Internship,
            WorkSchedule = WorkSchedule.PartTime,
            SeniorityLevel = SeniorityLevel.Beginner,
            City = "Belgrade",
            Country = "Serbija",
            WorkType = WorkType.Hybrid,
            SalaryFrom = 1000,
            SalaryTo = 2000,
            IsSalaryVisible = true
        };
        
        JobAdvertisement? capturedAd = null;

        _dbSetMock
            .Setup(d => d.Add(It.IsAny<JobAdvertisement>()))
            .Callback<JobAdvertisement>(ad =>
            {
                ad.Id = 123; 
                capturedAd = ad;
            });

        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

      
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(123);

        capturedAd.Should().NotBeNull();
        capturedAd!.Title.Should().Be(command.Title);
        capturedAd.Status.Should().Be(JobPostingStatus.Draft);
        capturedAd.IsActive.Should().BeTrue();
        capturedAd.City.Should().Be(command.City);

        _dbSetMock.Verify(d => d.Add(It.IsAny<JobAdvertisement>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}