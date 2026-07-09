using FluentValidation;

namespace JobLess.JobApplication.Application.Queries.GetCompanyApplications;

public class GetCompanyApplicationsQueryValidator : AbstractValidator<GetCompanyApplicationsQuery>
{
    public GetCompanyApplicationsQueryValidator()
    {
        RuleFor(x => x.CompanyEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.AdvertisementId)
            .GreaterThan(0)
            .When(x => x.AdvertisementId.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);
    }
}
