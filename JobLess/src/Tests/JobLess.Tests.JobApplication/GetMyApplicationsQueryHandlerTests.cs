using FluentAssertions;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Queries.GetMyApplications;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class GetMyApplicationsQueryHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly Mock<IAdvertisementLookupService> _adLookupMock;
    private readonly GetMyApplicationsQueryHandler _handler;

    public GetMyApplicationsQueryHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _adLookupMock = new Mock<IAdvertisementLookupService>();
        _handler = new GetMyApplicationsQueryHandler(_contextMock.Object, _adLookupMock.Object);

        _adLookupMock
            .Setup(s => s.GetTitlesByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, string>());
    }

    [Fact]
    public async Task Vraca_prijave_ulogovanog_kandidata()
    {
        var mine = JobApplicationTestHelper.CreateApplication(id: 1, candidateEmail: "marko.petrovic@email.rs");
        var other = JobApplicationTestHelper.CreateApplication(
            id: 2,
            candidateId: 8,
            candidateEmail: "ana.anic@email.rs",
            advertisementId: 11);

        var dbSet = new[] { mine, other }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);

        var result = await _handler.Handle(
            new GetMyApplicationsQuery("marko.petrovic@email.rs"),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].CandidateEmail.Should().Be("marko.petrovic@email.rs");
        result[0].AdvertisementTitle.Should().Be("Junior .NET Developer");
    }

    [Fact]
    public async Task Vraca_praznu_listu_ako_kandidat_nema_prijava()
    {
        var dbSet = Array.Empty<JobApplicationEntity>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSet.Object);

        var result = await _handler.Handle(
            new GetMyApplicationsQuery("novi.kandidat@email.rs"),
            CancellationToken.None);

        result.Should().BeEmpty();
    }
}
