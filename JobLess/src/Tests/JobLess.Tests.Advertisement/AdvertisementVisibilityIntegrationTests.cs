using FluentAssertions;
using JobLess.Advertisement.Application.Commands.Create;
using JobLess.Advertisement.Application.Interfaces;
using JobLess.Advertisement.Application.Queries.GetAll;
using JobLess.Advertisement.Application.Queries.GetAllForCompany;
using JobLess.Advertisement.Domain.Entities;
using JobLess.Advertisement.Domain.Enums;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.Advertisement;

public class AdvertisementVisibilityIntegrationTests
{
    private readonly Mock<IApplicationDbContext> _contextMock = new ();
    private readonly List<JobAdvertisement> _advertisements = new ();

    private readonly CreateAdvertisementCommandHandler _createHandler;
    private readonly GetAllForComapnyQueryHandler _getForCompanyHandler;
    private readonly GetAllAdvertisementQueryHandler _getAllHandler;

    public AdvertisementVisibilityIntegrationTests()
    {
        _createHandler = new CreateAdvertisementCommandHandler(_contextMock.Object);
        _getForCompanyHandler = new GetAllForComapnyQueryHandler(_contextMock.Object);
        _getAllHandler = new GetAllAdvertisementQueryHandler(_contextMock.Object);

        RefreshDbSet();
    }

    private void RefreshDbSet()
    {
        var dbSetMock = _advertisements.AsQueryable().BuildMockDbSet();

        _contextMock
            .Setup(c => c.JobAdvertisements)
            .Returns(dbSetMock.Object);

        _contextMock
            .Setup(c => c.JobAdvertisements.Add(It.IsAny<JobAdvertisement>()))
            .Callback<JobAdvertisement>(advertisement =>
            {
                advertisement.Id = _advertisements.Count + 1;
                _advertisements.Add(advertisement);
            });

        _contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private static CreateAdvertisementCommand CreateAdvertisement(
        int companyId,
        string companyEmail,
        string title) => new ()
        {
            CompanyId = companyId,
            CompanyEmail = companyEmail,
            Title = title,
            Description = $"Opis za {title}",
            Position = "Developer",
            EmploymentType = EmploymentType.Permanent,
            WorkSchedule = WorkSchedule.Full,
            SeniorityLevel = SeniorityLevel.Medior,
            City = "Beograd",
            Country = "Srbija",
            WorkType = WorkType.Hybrid,
            SalaryFrom = 1500,
            SalaryTo = 2500,
            IsSalaryVisible = true,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

    [Fact]
    public async Task Should_Return_Advertisements_Only_For_Owning_Company_And_All_For_Candidates()
    {
        const string companyOneEmail = "hr@kompanija1.rs";
        const string companyTwoEmail = "hr@kompanija2.rs";

        var firstAdvertisementId = await _createHandler.Handle(
            CreateAdvertisement(
                companyId: 1,
                companyEmail: companyOneEmail,
                title: "Backend Developer — Kompanija1"),
            CancellationToken.None);

        RefreshDbSet();

        var secondAdvertisementId = await _createHandler.Handle(
            CreateAdvertisement(
                companyId: 2,
                companyEmail: companyTwoEmail,
                title: "Frontend Developer — Kompanija2"),
            CancellationToken.None);

        RefreshDbSet();

        firstAdvertisementId.Should().NotBe(secondAdvertisementId);

        var companyOneAdvertisements = await _getForCompanyHandler.Handle(
            new GetAllForCompanyQuery
            {
                CompanyId = 1,
                CompanyEmail = companyOneEmail,
                PageNumber = 1,
                PageSize = 10
            },
            CancellationToken.None);

        companyOneAdvertisements.Advertisements.Should().HaveCount(1);
        companyOneAdvertisements.Advertisements.Single().Id.Should().Be(firstAdvertisementId);
        companyOneAdvertisements.Advertisements.Single().CompanyId.Should().Be(1);
        companyOneAdvertisements.Advertisements.Should().NotContain(a => a.Id == secondAdvertisementId);

        var companyTwoAdvertisements = await _getForCompanyHandler.Handle(
            new GetAllForCompanyQuery
            {
                CompanyId = 2,
                CompanyEmail = companyTwoEmail,
                PageNumber = 1,
                PageSize = 10
            },
            CancellationToken.None);

        companyTwoAdvertisements.Advertisements.Should().HaveCount(1);
        companyTwoAdvertisements.Advertisements.Single().Id.Should().Be(secondAdvertisementId);
        companyTwoAdvertisements.Advertisements.Single().CompanyId.Should().Be(2);
        companyTwoAdvertisements.Advertisements.Should().NotContain(a => a.Id == firstAdvertisementId);

        var candidateAdvertisements = await _getAllHandler.Handle(
            new GetAllAdvertisementQuery
            {
                PageNumber = 1,
                PageSize = 10
            },
            CancellationToken.None);

        candidateAdvertisements.Advertisements.Should().HaveCount(2);
        candidateAdvertisements.Advertisements
            .Select(a => a.Id)
            .Should()
            .BeEquivalentTo(new[] { firstAdvertisementId, secondAdvertisementId });
    }
}