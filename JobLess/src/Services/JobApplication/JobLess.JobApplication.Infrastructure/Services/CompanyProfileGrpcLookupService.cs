using Grpc.Core;
using JobLess.Grpc.Contracts;
using JobLess.JobApplication.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JobLess.JobApplication.Infrastructure.Services;

public class CompanyProfileGrpcLookupService(
    CompanyProfileGrpc.CompanyProfileGrpcClient client,
    IHttpContextAccessor httpContextAccessor) : ICompanyLookupService
{
    public async Task<CompanyLookupResult?> GetByIdAsync(
        int companyId,
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
            var reply = await client.GetByIdAsync(
                new GetByIdRequest { CompanyId = companyId },
                headers,
                cancellationToken: cancellationToken);

            if (reply is null || !reply.Found || string.IsNullOrWhiteSpace(reply.Email))
            {
                return null;
            }

            return new CompanyLookupResult(reply.CompanyId, reply.Email);
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
