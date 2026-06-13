using JobLess.Client.Application.Clients.Commands.CreateClientProfile;
using JobLess.Client.Application.Clients.Commands.UpdateClientProfile;
using JobLess.Client.Application.Clients.Queries.GetClientProfile;
using JobLess.Client.Application.Clients.Queries.GetClientProfileByEmail;
using JobLess.Client.Application.Models;
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
}
