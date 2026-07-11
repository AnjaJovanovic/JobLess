using FluentAssertions;
using JobLess.JobApplication.Application.Interfaces;
using JobLess.JobApplication.Application.Queries.GetCompanyApplications;
using JobLess.JobApplication.Domain.Enums;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;
using MockQueryable.Moq;
using Moq;

namespace JobLess.Tests.JobApplication;

public class GetCompanyApplicationsQueryHandlerTests
{
    private readonly Mock<IJobApplicationDbContext> _contextMock;
    private readonly GetCompanyApplicationsQueryHandler _handler;

    public GetCompanyApplicationsQueryHandlerTests()
    {
        _contextMock = new Mock<IJobApplicationDbContext>();
        _handler = new GetCompanyApplicationsQueryHandler(_contextMock.Object);
    }

    private void SetupApplicationsDbSet(List<JobApplicationEntity> applications)
    {
        var dbSetMock = applications.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.JobApplications).Returns(dbSetMock.Object);
    }

    [Fact]
    public async Task Vraca_prijave_za_kompaniju()
    {
        SetupApplicationsDbSet(
        [
            JobApplicationTestHelper.CreatePendingApplication(id: 1, companyEmail: "firma@kompanija.rs"),
            JobApplicationTestHelper.CreatePendingApplication(id: 2, companyEmail: "druga@kompanija.rs"),
            JobApplicationTestHelper.CreatePendingApplication(id: 3, companyEmail: "firma@kompanija.rs")
        ]);

        var result = await _handler.Handle(
            new GetCompanyApplicationsQuery("FIRMA@kompanija.rs"),
            CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.CompanyEmail == "firma@kompanija.rs");
    }

    [Fact]
    public async Task Filtrira_po_advertisement_id()
    {
        SetupApplicationsDbSet(
        [
            JobApplicationTestHelper.CreatePendingApplication(id: 1, advertisementId: 10, companyEmail: "firma@kompanija.rs"),
            JobApplicationTestHelper.CreatePendingApplication(id: 2, advertisementId: 11, companyEmail: "firma@kompanija.rs")
        ]);

        var result = await _handler.Handle(
            new GetCompanyApplicationsQuery("firma@kompanija.rs", AdvertisementId: 10),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].AdvertisementId.Should().Be(10);
    }

    [Fact]
    public async Task Filtrira_po_statusu()
    {
        var accepted = JobApplicationTestHelper.CreatePendingApplication(id: 1, companyEmail: "firma@kompanija.rs");
        accepted.Accept();
        var pending = JobApplicationTestHelper.CreatePendingApplication(id: 2, companyEmail: "firma@kompanija.rs");

        SetupApplicationsDbSet([accepted, pending]);

        var result = await _handler.Handle(
            new GetCompanyApplicationsQuery("firma@kompanija.rs", Status: JobApplicationStatus.Accepted),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be((int)JobApplicationStatus.Accepted);
    }
}
