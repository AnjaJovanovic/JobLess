using ClientEntity = JobLess.Client.Domain.Entities.Client;
using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Mappings;
using JobLess.Client.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Commands.CreateClientProfile;

public class CreateClientProfileCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateClientProfileCommand, ClientProfileDto>
{
    public async Task<ClientProfileDto> Handle(CreateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await context.Clients
            .AnyAsync(c => c.Email.ToLower() == email, cancellationToken);

        if (emailExists)
            throw new InvalidOperationException("Profil za ovaj email već postoji.");

        var client = new ClientEntity
        {
            Email = email,
            PasswordHash = string.Empty,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        ClientProfileMapper.ApplyProfileFields(
            client,
            request.DateOfBirth,
            request.City,
            request.Address,
            request.EducationLevel,
            request.InstitutionName,
            request.EducationStartYear,
            request.EducationEndYear,
            request.YearsOfExperience,
            request.Skills,
            request.ProfessionalSummary,
            request.LinkedInUrl);

        context.Clients.Add(client);
        await context.SaveChangesAsync(cancellationToken);

        return ClientProfileMapper.ToDto(client);
    }
}
