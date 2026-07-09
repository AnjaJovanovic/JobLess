using FluentValidation;

namespace JobLess.JobApplication.Application.Queries.GetMyApplications;

public class GetMyApplicationsQueryValidator : AbstractValidator<GetMyApplicationsQuery>
{
    public GetMyApplicationsQueryValidator()
    {
        RuleFor(x => x.CandidateEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
