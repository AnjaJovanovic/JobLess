using FluentValidation;

namespace JobLess.Client.Application.Clients.Commands.CreateClientProfile;

public class CreateClientProfileCommandValidator : AbstractValidator<CreateClientProfileCommand>
{
    public CreateClientProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(40);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30)
            .When(x => x.PhoneNumber is not null);

        RuleFor(x => x.Gender)
            .IsInEnum();
    }
}
