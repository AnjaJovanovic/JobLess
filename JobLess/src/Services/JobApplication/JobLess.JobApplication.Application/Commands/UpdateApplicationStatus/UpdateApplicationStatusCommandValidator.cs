using FluentValidation;
using JobLess.JobApplication.Domain.Enums;

namespace JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;

public class UpdateApplicationStatusCommandValidator : AbstractValidator<UpdateApplicationStatusCommand>
{
    public UpdateApplicationStatusCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);

        RuleFor(x => x.CompanyEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Status)
            .Must(status => status is JobApplicationStatus.Accepted or JobApplicationStatus.Rejected)
            .WithMessage("Status mora biti Accepted ili Rejected.");
    }
}
