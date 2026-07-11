using System.Security.Claims;
using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
using JobLess.Client.Application.Clients.Queries.GetClientProfile;
using JobLess.Client.Application.Clients.Queries.GetClientProfileByEmail;
using JobLess.Client.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobLess.Client.API.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
public class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpPut("profile")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<ClientProfileDto>> CreateProfile(
        [FromBody] ClientProfileDto request,
        CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized();

        try
        {
            var result = await mediator.Send(
                new CreateClientProfileCommand(
                    email,
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
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<ClientProfileDto>> GetProfileByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = "Email je obavezan." });

        var tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(tokenEmail))
            return Unauthorized();

        if (!string.Equals(tokenEmail.Trim(), email.Trim(), StringComparison.OrdinalIgnoreCase))
            return Forbid();

        var result = await mediator.Send(new GetClientProfileByEmailQuery(email), cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:int}/profile")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<ClientProfileDto>> UpdateProfile(
        int id,
        [FromBody] ClientProfileDto request,
        CancellationToken cancellationToken)
    {
        var tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(tokenEmail))
            return Unauthorized();

        var existing = await mediator.Send(new GetClientProfileQuery(id), cancellationToken);
        if (existing is null)
            return NotFound();

        if (!string.Equals(existing.Email, tokenEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            return Forbid();

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
    [Authorize(Roles = "Candidate,Company")]
    public async Task<ActionResult<ClientProfileDto>> GetProfile(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientProfileQuery(id), cancellationToken);

        if (result is null)
            return NotFound();

        if (User.IsInRole("Candidate"))
        {
            var tokenEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(tokenEmail))
                return Unauthorized();

            if (!string.Equals(result.Email, tokenEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                return Forbid();
        }

        return Ok(result);
    }
}
