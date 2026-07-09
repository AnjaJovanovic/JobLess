using System.Security.Claims;
using JobLess.JobApplication.API.Contracts;
using JobLess.JobApplication.Application.Commands.ApplyForJob;
using JobLess.JobApplication.Application.Commands.UpdateApplicationStatus;
using JobLess.JobApplication.Application.Models;
using JobLess.JobApplication.Application.Queries.GetCompanyApplications;
using JobLess.JobApplication.Application.Queries.GetMyApplications;
using JobLess.JobApplication.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobLess.JobApplication.API.Controllers;

[ApiController]
[Route("api/job-applications")]
[Authorize]
public class JobApplicationsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<JobApplicationDto>> Apply(
        [FromBody] ApplyForJobRequest request,
        CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        try
        {
            var result = await mediator.Send(new ApplyForJobCommand(
                request.AdvertisementId,
                request.CompanyId,
                email), cancellationToken);

            return CreatedAtAction(nameof(GetMyApplications), result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationDto>>> GetMyApplications(CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(new GetMyApplicationsQuery(email), cancellationToken);
        return Ok(result);
    }

    [HttpGet("company")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<IReadOnlyList<JobApplicationDto>>> GetCompanyApplications(
        [FromQuery] int? advertisementId,
        [FromQuery] JobApplicationStatus? status,
        CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        var result = await mediator.Send(new GetCompanyApplicationsQuery(
            email,
            advertisementId,
            status), cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Company")]
    public async Task<ActionResult<JobApplicationDto>> UpdateStatus(
        int id,
        [FromBody] UpdateApplicationStatusRequest request,
        CancellationToken cancellationToken)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        try
        {
            var result = await mediator.Send(new UpdateApplicationStatusCommand(
                id,
                email,
                (JobApplicationStatus)request.Status), cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { service = "JobApplication", status = "ok" });
    }
}
