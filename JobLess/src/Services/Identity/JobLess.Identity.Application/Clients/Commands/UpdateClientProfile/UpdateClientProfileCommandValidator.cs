using FluentValidation;

namespace JobLess.Identity.Application.Clients.Commands.UpdateClientProfile;

public class UpdateClientProfileCommandValidator : AbstractValidator<UpdateClientProfileCommand>
{
    public UpdateClientProfileCommandValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .When(x => x.PhoneNumber is not null);
    }
}
