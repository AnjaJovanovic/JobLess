using FluentAssertions;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Queries.GetCompanyApplications;
using JobLess.JobApplication.Domain.Enums;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class GetCompanyApplicationsQueryHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly Mock<IAdvertisementLookupService> _adLookupMock;
    private readonly GetCompanyApplicationsQueryHandler _handler;

    public GetCompanyApplicationsQueryHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _adLookupMock = new Mock<IAdvertisementLookupService>();
        _handler = new GetCompanyApplicationsQueryHandler(_contextMock.Object, _adLookupMock.Object);

        _adLookupMock
            .Setup(s => s.GetTitlesByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, string>());
    }

    [Fact]
    public async Task Vraca_prijave_za_kompaniju()
    {
        var app1 = JobApplicationTestHelper.CreateApplication(id: 1, companyEmail: "hr@tehnobit.rs");
        var app2 = JobApplicationTestHelper.CreateApplication(
            id: 2,
            companyId: 9,
            companyEmail: "kontakt@druga.rs",
            advertisementId: 20);

        var dbSet = new[] { app1, app2 }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);

        var result = await _handler.Handle(
            new GetCompanyApplicationsQuery("hr@tehnobit.rs"),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].CompanyEmail.Should().Be("hr@tehnobit.rs");
    }

    [Fact]
    public async Task Filtrira_prijave_po_statusu()
    {
        var pending = JobApplicationTestHelper.CreateApplication(id: 1);
        var accepted = JobApplicationTestHelper.CreateApplication(id: 2, advertisementId: 11);
        accepted.Accept();

        var dbSet = new[] { pending, accepted }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);

        var result = await _handler.Handle(
            new GetCompanyApplicationsQuery("hr@tehnobit.rs", Status: JobApplicationStatus.Accepted),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be((int)JobApplicationStatus.Accepted);
    }

    [Fact]
    public async Task Filtrira_prijave_po_oglasu()
    {
        var forAd10 = JobApplicationTestHelper.CreateApplication(id: 1, advertisementId: 10);
        var forAd20 = JobApplicationTestHelper.CreateApplication(id: 2, advertisementId: 20);

        var dbSet = new[] { forAd10, forAd20 }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);

        var result = await _handler.Handle(
            new GetCompanyApplicationsQuery("hr@tehnobit.rs", AdvertisementId: 10),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].AdvertisementId.Should().Be(10);
    }
}
