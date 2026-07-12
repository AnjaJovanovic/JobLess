using Grpc.Core;
using JobLess.Grpc.Contracts;
using JobLess.JobApplication.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobLess.JobApplication.Infrastructure.Services;

public class ClientProfileGrpcLookupService(
    ClientProfileGrpc.ClientProfileGrpcClient client,
    IHttpContextAccessor httpContextAccessor) : IClientProfileLookupService
{
    public async Task<ClientProfileLookupResult?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var headers = new Metadata();
        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            headers.Add("Authorization", authorization);
        }

        try
        {
            var reply = await client.GetByEmailAsync(
                new GetByEmailRequest { Email = email },
                headers,
                cancellationToken: cancellationToken);

            if (reply is null || !reply.Found)
            {
                return null;
            }

            return new ClientProfileLookupResult(
                reply.ClientId,
                reply.FirstName,
                reply.LastName,
                reply.Email);
        }
        catch (RpcException ex) when (
            ex.StatusCode is StatusCode.NotFound
                or StatusCode.Unauthenticated
                or StatusCode.PermissionDenied)
        {
            return null;
        }
    }
}
