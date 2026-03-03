using FluentAssertions;
using JobLess.Advertisement.Application.Commands.Delete;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobLess.Tests.Advertisement
{
    public class DeleteAdvertisementCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IValidationExceptionThrower> _validationThrowerMock;
        private readonly DeleteAdvertisementCommandHandler _handler;

        public DeleteAdvertisementCommandHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _validationThrowerMock = new Mock<IValidationExceptionThrower>();
            _handler = new DeleteAdvertisementCommandHandler(_contextMock.Object, _validationThrowerMock.Object);
        }

        private JobAdvertisement CreateAd(bool isActive) => new JobAdvertisement
        {
            Id = 1,
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
            IsSalaryVisible = true,
            Status = JobPostingStatus.Draft,
            IsActive = isActive,
            PostedAt = System.DateTime.UtcNow
        };

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
        public async Task Handle_Should_Deactivate_Advertisement_When_Exists_And_Active()
        {
            // Arrange
            var ad = CreateAd(isActive: true);
            SetupDbSet(new List<JobAdvertisement> { ad });

            var command = new DeleteAdvertismentCommand { Id = 1 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            ad.IsActive.Should().BeFalse();
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Advertisement_Already_Inactive()
        {
            // Arrange
            var ad = CreateAd(isActive: false);
            SetupDbSet(new List<JobAdvertisement> { ad });

            _validationThrowerMock
                .Setup(v => v.ThrowValidationException("Id", "Advertisement does not exist or has already been deactivated."))
                .Throws(new Exception("Advertisement does not exist or has already been deactivated."));

            var command = new DeleteAdvertismentCommand { Id = 1 };

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Advertisement does not exist or has already been deactivated.");

            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Advertisement_Does_Not_Exist()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>());

            _validationThrowerMock
                .Setup(v => v.ThrowValidationException("Id", "Advertisement does not exist or has already been deactivated."))
                .Throws(new Exception("Advertisement does not exist or has already been deactivated."));

            var command = new DeleteAdvertismentCommand { Id = 999 };

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Advertisement does not exist or has already been deactivated.");

            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}