using JobLess.Client.Application.Interfaces;
using JobLess.Client.Application.Mappings;
using JobLess.Client.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobLess.Client.Application.Clients.Commands.UpdateClientProfile;

public class UpdateClientProfileCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateClientProfileCommand, ClientProfileDto>
{
    public async Task<ClientProfileDto> Handle(UpdateClientProfileCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Clients
            .FirstOrDefaultAsync(c => c.ClientId == request.ClientId, cancellationToken);

        if (client is null)
            throw new KeyNotFoundException($"Klijent sa ID {request.ClientId} nije pronađen.");

        client.FirstName = request.FirstName;
        client.LastName = request.LastName;
        client.PhoneNumber = request.PhoneNumber;
        client.Gender = request.Gender;
        client.IsActive = request.IsActive;

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

        await context.SaveChangesAsync(cancellationToken);

        return ClientProfileMapper.ToDto(client);
    }
}
