using JobApplicationEntity = JobLess.JobApplication.Domain.Entities.JobApplication;

namespace JobLess.Tests.JobApplication;

internal static class JobApplicationTestHelper
{
    public static JobApplicationEntity CreateApplication(
        int id = 1,
        int advertisementId = 10,
        string advertisementTitle = "Junior .NET Developer",
        int candidateId = 5,
        string candidateEmail = "marko.petrovic@email.rs",
        string candidateFirstName = "Marko",
        string candidateLastName = "Petrović",
        int companyId = 2,
        string companyEmail = "hr@tehnobit.rs")
    {
        var application = JobApplicationEntity.Create(
            advertisementId,
            advertisementTitle,
            candidateId,
            candidateEmail,
            candidateFirstName,
            candidateLastName,
            companyId,
            companyEmail);

        typeof(JobApplicationEntity)
            .GetProperty(nameof(JobApplicationEntity.Id))!
            .SetValue(application, id);

        return application;
    }
}
