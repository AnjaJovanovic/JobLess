using JobLess.IdentityServer.Application.Commands.LoginUser;
using JobLess.IdentityServer.Application.Commands.RegisterUser;
using JobLess.IdentityServer.Application.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobLess.IdentityServer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result is null)
            return Unauthorized(new { message = "Pogrešni kredencijali." });
        return Ok(result);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        if (result is null)
            return Unauthorized(new { message = "Nevažeći ili istekli refresh token." });
        return Ok(result);
    }

}
