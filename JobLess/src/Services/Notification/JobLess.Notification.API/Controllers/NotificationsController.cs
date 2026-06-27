using System.Security.Claims;
using JobLess.Notification.Application.Commands.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationEntity = JobLess.Notification.Domain.Entities.Notification;

namespace JobLess.Notification.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    private string? GetAuthUserEmail()
    {
        var emailClaim = User.FindFirst(ClaimTypes.Email) ?? User.FindFirst("email");
        return emailClaim?.Value;
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<NotificationEntity>>> GetMyNotifications(CancellationToken cancellationToken)
    {
        var userEmail = GetAuthUserEmail();
        if (userEmail == null)
            return Unauthorized();

        var result = await mediator.Send(new GetUserNotificationsCommand(userEmail), cancellationToken);
        return Ok(result);
    }
}
