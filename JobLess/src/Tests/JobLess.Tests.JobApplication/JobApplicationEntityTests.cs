using FluentAssertions;
using JobLess.JobApplication.Domain.Enums;
using JobLess.JobApplication.Domain.Exceptions;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;

namespace JobLess.Tests.JobApplication;

public class JobApplicationEntityTests
{
    [Fact]
    public void Nova_prijava_ima_pending_status()
    {
        var application = JobApplicationEntity.Create(
            10, "Junior .NET Developer", 5,
            "marko.petrovic@email.rs", "Marko", "Petrović",
            2, "hr@tehnobit.rs");

        application.Status.Should().Be(JobApplicationStatus.Pending);
        application.AdvertisementTitle.Should().Be("Junior .NET Developer");
    }

    [Fact]
    public void Accept_menja_status_u_accepted()
    {
        var application = JobApplicationTestHelper.CreateApplication();

        application.Accept();

        application.Status.Should().Be(JobApplicationStatus.Accepted);
        application.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_menja_status_u_rejected()
    {
        var application = JobApplicationTestHelper.CreateApplication();

        application.Reject();

        application.Status.Should().Be(JobApplicationStatus.Rejected);
    }

    [Fact]
    public void Ne_moze_dva_puta_da_se_prihvati_ista_prijava()
    {
        var application = JobApplicationTestHelper.CreateApplication();
        application.Accept();

        var act = () => application.Accept();

        act.Should().Throw<InvalidJobApplicationStatusTransitionException>();
    }
}
