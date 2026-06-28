using JobLess.Client.Application.Clients.Commands.ApplyToJob;
using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
using JobLess.Client.Application.Clients.Commands.UpdateJobApplicationStatus;
using JobLess.Client.Application.Clients.Queries.GetApplicationsByAdvertisements;
using JobLess.Client.Application.Clients.Queries.GetClientApplications;
using JobLess.Client.Application.Clients.Queries.GetClientProfile;
using JobLess.Client.Application.Clients.Queries.GetClientProfileByEmail;
using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobLess.Client.API.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpPut("profile")]
    public async Task<ActionResult<ClientProfileDto>> CreateProfile(
        [FromBody] ClientProfileDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(
                new CreateClientProfileCommand(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.Gender,
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
                    request.LinkedInUrl,
                    request.IsActive),
                cancellationToken);

            return CreatedAtAction(nameof(GetProfile), new { id = result.ClientId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("profile/by-email")]
    public async Task<ActionResult<ClientProfileDto>> GetProfileByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = "Email je obavezan." });

        var result = await mediator.Send(new GetClientProfileByEmailQuery(email), cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:int}/profile")]
    public async Task<ActionResult<ClientProfileDto>> UpdateProfile(
        int id,
        [FromBody] ClientProfileDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(
                    new UpdateClientProfileCommand(
                    id,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.Gender,
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
                    request.LinkedInUrl,
                    request.IsActive),
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:int}/profile")]
    public async Task<ActionResult<ClientProfileDto>> GetProfile(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientProfileQuery(id), cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{id:int}/applications")]
    public async Task<ActionResult<JobApplicationDto>> ApplyToJob(
        int id,
        [FromBody] ApplyToJobRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(
                new ApplyToJobCommand(id, request.AdvertisementId, request.CompanyEmail ?? string.Empty),
                cancellationToken);

            return CreatedAtAction(nameof(GetApplications), new { id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/applications")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationDto>>> GetApplications(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientApplicationsQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("applications/by-advertisements")]
    public async Task<ActionResult<IReadOnlyList<CompanyApplicationDto>>> GetApplicationsByAdvertisements(
        [FromQuery] int[] advertisementIds,
        [FromQuery] JobApplicationStatus? status,
        CancellationToken cancellationToken)
    {
        var ids = advertisementIds ?? Array.Empty<int>();
        var result = await mediator.Send(
            new GetApplicationsByAdvertisementsQuery(ids, status),
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("applications/{applicationId:int}/status")]
    public async Task<ActionResult<JobApplicationDto>> UpdateApplicationStatus(
        int applicationId,
        [FromBody] UpdateApplicationStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(
                new UpdateJobApplicationStatusCommand(applicationId, request.Status),
                cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

public record ApplyToJobRequest(int AdvertisementId, string? CompanyEmail);

public record UpdateApplicationStatusRequest(JobApplicationStatus Status);
