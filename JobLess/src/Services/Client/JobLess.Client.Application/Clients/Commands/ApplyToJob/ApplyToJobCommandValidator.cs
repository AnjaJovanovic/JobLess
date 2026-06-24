using FluentValidation;

namespace JobLess.Client.Application.Clients.Commands.ApplyToJob;

public class ApplyToJobCommandValidator : AbstractValidator<ApplyToJobCommand>
{
    public ApplyToJobCommandValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.AdvertisementId).GreaterThan(0);
    }
}
