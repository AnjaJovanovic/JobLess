using FluentAssertions;
using JobLess.JobApplication.Application.Queries.GetMyApplications;
using JobLess.JobApplication.Application.Interfaces;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class GetMyApplicationsQueryHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly GetMyApplicationsQueryHandler _handler;

    public GetMyApplicationsQueryHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _handler = new GetMyApplicationsQueryHandler(_contextMock.Object);
    }

    private void SetupApplicationsDbSet(List<JobApplicationEntity> applications)
    {
        var dbSetMock = applications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSetMock.Object);
    }

    [Fact]
    public async Task Vraca_prijave_za_email_kandidata()
    {
        SetupApplicationsDbSet(
        [
            JobApplicationTestHelper.CreatePendingApplication(
                id: 1,
                advertisementId: 10,
                candidateEmail: "marko.petrovic@email.rs"),
            JobApplicationTestHelper.CreatePendingApplication(
                id: 2,
                advertisementId: 11,
                candidateEmail: "ana.anic@email.rs"),
            JobApplicationTestHelper.CreatePendingApplication(
                id: 3,
                advertisementId: 12,
                candidateEmail: "marko.petrovic@email.rs")
        ]);

        var result = await _handler.Handle(
            new GetMyApplicationsQuery("MARKO.PETROVIC@email.rs"),
            CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.CandidateEmail == "marko.petrovic@email.rs");
        result[0].Id.Should().Be(3);
        result[1].Id.Should().Be(1);
    }

    [Fact]
    public async Task Vraca_praznu_listu_kada_nema_prijava()
    {
        SetupApplicationsDbSet([]);

        var result = await _handler.Handle(
            new GetMyApplicationsQuery("marko.petrovic@email.rs"),
            CancellationToken.None);

        result.Should().BeEmpty();
    }
}
