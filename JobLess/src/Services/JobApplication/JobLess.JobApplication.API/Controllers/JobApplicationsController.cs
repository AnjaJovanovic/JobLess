using System.Security.Claims;
using JobLess.JobApplication.API.Contracts;
using JobLess.JobApplication.Application.Commands.ApplyForJob;
using JobLess.JobApplication.Application.Models;
using JobLess.JobApplication.Application.Queries.GetMyApplications;
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

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { service = "JobApplication", status = "ok" });
    }
}
