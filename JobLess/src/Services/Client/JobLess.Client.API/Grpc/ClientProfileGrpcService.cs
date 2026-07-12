using System.Security.Claims;
using Grpc.Core;
using JobLess.Client.Application.Clients.Queries.GetClientProfileByEmail;
using JobLess.Grpc.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace JobLess.Client.API.Grpc;

[Authorize(Roles = "Candidate")]
public class ClientProfileGrpcService(IMediator mediator) : ClientProfileGrpc.ClientProfileGrpcBase
{
    public override async Task<ClientProfileReply> GetByEmail(
        GetByEmailRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Email je obavezan."));
        }

        var httpContext = context.GetHttpContext();
        var tokenEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrWhiteSpace(tokenEmail))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Niste autentifikovani."));
        }

        if (!string.Equals(tokenEmail.Trim(), request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Nemate dozvolu za ovaj profil."));
        }

        var profile = await mediator.Send(
            new GetClientProfileByEmailQuery(request.Email),
            context.CancellationToken);

        if (profile is null)
        {
            return new ClientProfileReply { Found = false };
        }

        return new ClientProfileReply
        {
            Found = true,
            ClientId = profile.ClientId,
            FirstName = profile.FirstName ?? string.Empty,
            LastName = profile.LastName ?? string.Empty,
            Email = profile.Email ?? string.Empty
        };
    }
}
