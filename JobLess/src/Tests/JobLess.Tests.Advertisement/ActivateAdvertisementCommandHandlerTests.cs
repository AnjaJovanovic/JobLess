using FluentAssertions;
using JobLess.Advertisement.Application.Commands.Activate;
using JobLess.Advertisement.Application.Commands.Delete;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ActivateAdvertisementCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IValidationExceptionThrower> _validationThrowerMock;
    private readonly AktivirajOglasHandler _handler;

    public ActivateAdvertisementCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _validationThrowerMock = new Mock<IValidationExceptionThrower>();
        _handler = new AktivirajOglasHandler(_contextMock.Object, _validationThrowerMock.Object);
    }

    private void SetupDbSet(List<JobAdvertisement> data)
    {
        var dbSetMock = data.AsQueryable().BuildMockDbSet();
        _contextMock
            .Setup(c => c.JobAdvertisements)
            .Returns(dbSetMock.Object);
        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    [Fact]
    public async Task Handle_Should_Activate_Advertisement_When_Exists_And_Inactive()
    {
        // Arrange
        var ad = new JobAdvertisement
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
            IsActive = false,
            PostedAt = System.DateTime.UtcNow
        };
        SetupDbSet(new List<JobAdvertisement> { ad });

        var command = new ActivateAdvertisementCommand { Id = 123 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        ad.IsActive.Should().BeTrue();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Remain_Active_When_Advertisement_Already_Active()
    {
        // Arrange
        var ad = new JobAdvertisement
        {
            Id = 124,
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
            PostedAt = System.DateTime.UtcNow
        };
        SetupDbSet(new List<JobAdvertisement> { ad });

        var command = new ActivateAdvertisementCommand { Id = 124 };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        ad.IsActive.Should().BeTrue();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Throw_When_Advertisement_Does_Not_Exist()
    {
        // Arrange
        SetupDbSet(new List<JobAdvertisement>());
        _validationThrowerMock
            .Setup(v => v.ThrowValidationException("Id", "Advertisement don't exists."))
            .Throws(new Exception("Advertisement don't exists."));

        var command = new ActivateAdvertisementCommand { Id = 999 };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Advertisement don't exists.");

        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}