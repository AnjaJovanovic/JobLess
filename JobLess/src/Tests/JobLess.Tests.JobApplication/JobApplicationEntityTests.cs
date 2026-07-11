using FluentAssertions;
using JobLess.JobApplication.Domain.Enums;
using JobLess.JobApplication.Domain.Exceptions;
using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;

namespace JobLess.Tests.JobApplication;

public class JobApplicationEntityTests
{
    [Fact]
    public void Kreira_prijavu_sa_ispravnim_podacima()
    {
        var application = JobApplicationEntity.Create(
            advertisementId: 10,
            candidateId: 5,
            candidateEmail: "marko.petrovic@email.rs",
            candidateFirstName: "Marko",
            candidateLastName: "Petrović",
            companyId: 3,
            companyEmail: "firma@kompanija.rs");

        application.AdvertisementId.Should().Be(10);
        application.CandidateId.Should().Be(5);
        application.CandidateEmail.Should().Be("marko.petrovic@email.rs");
        application.CompanyId.Should().Be(3);
        application.CompanyEmail.Should().Be("firma@kompanija.rs");
        application.Status.Should().Be(JobApplicationStatus.Pending);
        application.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        application.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(0, 1, 1)]
    [InlineData(1, 0, 1)]
    [InlineData(1, 1, 0)]
    public void Baca_izuzetak_za_nevalidne_id_vrednosti(int advertisementId, int candidateId, int companyId)
    {
        var act = () => JobApplicationEntity.Create(
            advertisementId,
            candidateId,
            "marko@email.rs",
            "Marko",
            "Petrović",
            companyId,
            "firma@kompanija.rs");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Prihvata_prijavu_kada_je_pending()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication();

        application.Accept();

        application.Status.Should().Be(JobApplicationStatus.Accepted);
        application.UpdatedAt.Should().NotBeNull();
        application.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Odbija_prijavu_kada_je_pending()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication();

        application.Reject();

        application.Status.Should().Be(JobApplicationStatus.Rejected);
        application.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Ne_dozvoljava_prihvatanje_vec_obradjene_prijave()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication();
        application.Accept();

        var act = () => application.Accept();

        act.Should().Throw<InvalidJobApplicationStatusTransitionException>()
            .Which.CurrentStatus.Should().Be(JobApplicationStatus.Accepted);
    }

    [Fact]
    public void Ne_dozvoljava_odbijanje_vec_obradjene_prijave()
    {
        var application = JobApplicationTestHelper.CreatePendingApplication();
        application.Reject();

        var act = () => application.Reject();

        act.Should().Throw<InvalidJobApplicationStatusTransitionException>()
            .Which.CurrentStatus.Should().Be(JobApplicationStatus.Rejected);
    }
}
