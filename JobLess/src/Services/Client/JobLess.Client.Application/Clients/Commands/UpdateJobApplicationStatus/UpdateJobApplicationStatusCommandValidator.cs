using FluentValidation;
using JobLess.Client.Domain.Enums;

namespace JobLess.Client.Application.Clients.Commands.UpdateJobApplicationStatus;

public class UpdateJobApplicationStatusCommandValidator : AbstractValidator<UpdateJobApplicationStatusCommand>
{
    public UpdateJobApplicationStatusCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0)
            .WithMessage("ID prijave nije validan.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status prijave nije validan.");
    }
}
