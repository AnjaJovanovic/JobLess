using JobLess.JobApplication.Domain.Enums;

namespace JobLess.JobApplication.Domain.Exceptions;

public class InvalidJobApplicationStatusTransitionException : Exception
{
    public InvalidJobApplicationStatusTransitionException(JobApplicationStatus currentStatus, JobApplicationStatus targetStatus)
        : base($"Nije moguce promeniti status prijave iz '{currentStatus}' u '{targetStatus}'.")
    {
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }

    public JobApplicationStatus CurrentStatus { get; }
    public JobApplicationStatus TargetStatus { get; }
}
