using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;

namespace JobLess.Tests.JobApplication;

internal static class JobApplicationTestHelper
{
    public static JobApplicationEntity CreatePendingApplication(
        int id = 1,
        int advertisementId = 10,
        int candidateId = 5,
        string candidateEmail = "marko.petrovic@email.rs",
        string candidateFirstName = "Marko",
        string candidateLastName = "Petrović",
        int companyId = 3,
        string companyEmail = "firma@kompanija.rs")
    {
        var application = JobApplicationEntity.Create(
            advertisementId,
            candidateId,
            candidateEmail,
            candidateFirstName,
            candidateLastName,
            companyId,
            companyEmail);

        SetId(application, id);
        return application;
    }

    public static void SetId(JobApplicationEntity application, int id)
    {
        typeof(JobApplicationEntity)
            .GetProperty(nameof(JobApplicationEntity.Id))!
            .SetValue(application, id);
    }
}
