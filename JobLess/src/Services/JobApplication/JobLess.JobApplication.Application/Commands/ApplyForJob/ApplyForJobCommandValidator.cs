using FluentValidation;

namespace JobLess.JobApplication.Application.Commands.ApplyForJob;

public class ApplyForJobCommandValidator : AbstractValidator<ApplyForJobCommand>
{
    public ApplyForJobCommandValidator()
    {
        RuleFor(x => x.AdvertisementId).GreaterThan(0);
        RuleFor(x => x.CompanyId).GreaterThan(0);

        RuleFor(x => x.CandidateEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
