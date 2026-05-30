using FluentAssertions;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Queries.GetOne;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using JobLess.Shared.Domain.Common.Interfaces;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JobLess.Tests.Advertisement
{
    public class GetOneAdvertisementQueryHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _contextMock;
        private readonly Mock<IValidationExceptionThrower> _validationThrowerMock;
        private readonly GetOneAdvertisementQueryHandler _handler;

        public GetOneAdvertisementQueryHandlerTests()
        {
            _contextMock = new Mock<IApplicationDbContext>();
            _validationThrowerMock = new Mock<IValidationExceptionThrower>();
            _handler = new GetOneAdvertisementQueryHandler(_contextMock.Object, _validationThrowerMock.Object);
        }

        private void SetupDbSet(List<JobAdvertisement> data)
        {
            var dbSetMock = data.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(c => c.JobAdvertisements).Returns(dbSetMock.Object);
        }

        private JobAdvertisement CreateAd(int id, bool isActive, DateTime? expiresAt = null) => new JobAdvertisement
        {
            Id = id,
            CompanyId = 1,
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
        public async Task Handle_Should_Return_Advertisement_When_Exists_And_Active()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement> { CreateAd(1, isActive: true) });

            var query = new GetOneAdvertisementQuery { Id = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Advertisement.Id.Should().Be(1);
            result.Advertisement.Title.Should().Be("Backend Developer");
            result.Advertisement.IsActive.Should().BeTrue();
            result.Advertisement.City.Should().Be("Belgrade");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Advertisement_Does_Not_Exist()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>());

            _validationThrowerMock
                .Setup(v => v.ThrowValidationException("Id", "Advertisement does not exist or is not active."))
                .Throws(new Exception("Advertisement does not exist or is not active."));

            var query = new GetOneAdvertisementQuery { Id = 999 };

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Advertisement does not exist or is not active.");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Advertisement_Exists_But_Inactive()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement> { CreateAd(1, isActive: false) });

            _validationThrowerMock
                .Setup(v => v.ThrowValidationException("Id", "Advertisement does not exist or is not active."))
                .Throws(new Exception("Advertisement does not exist or is not active."));

            var query = new GetOneAdvertisementQuery { Id = 1 };

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Advertisement does not exist or is not active.");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Advertisement_Is_Expired()
        {
            // Arrange
            SetupDbSet(new List<JobAdvertisement>
            {
                CreateAd(1, isActive: true, expiresAt: DateTime.UtcNow.AddDays(-1))
            });

            _validationThrowerMock
                .Setup(v => v.ThrowValidationException("Id", "Advertisement does not exist or is not active."))
                .Throws(new Exception("Advertisement does not exist or is not active."));

            var query = new GetOneAdvertisementQuery { Id = 1 };

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Advertisement does not exist or is not active.");
        }
    }
}