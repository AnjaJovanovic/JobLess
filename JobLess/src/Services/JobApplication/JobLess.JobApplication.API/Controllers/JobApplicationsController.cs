using Microsoft.AspNetCore.Mvc;

namespace JobLess.JobApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobApplicationsController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { service = "JobApplication", status = "ok" });
    }
}
