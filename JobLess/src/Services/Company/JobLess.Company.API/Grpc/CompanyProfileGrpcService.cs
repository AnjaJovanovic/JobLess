using Grpc.Core;
using JobLess.Company.Application.Queries.GetOne;
using JobLess.Grpc.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace JobLess.Company.API.Grpc;

[Authorize]
public class CompanyProfileGrpcService(IMediator mediator) : CompanyProfileGrpc.CompanyProfileGrpcBase
{
    public override async Task<CompanyProfileReply> GetById(
        GetByIdRequest request,
        ServerCallContext context)
    {
        if (request.CompanyId <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "CompanyId mora biti pozitivan."));
        }

        var result = await mediator.Send(
            new GetOneCompanyQuery { Id = request.CompanyId },
            context.CancellationToken);

        var company = result?.Company;
        if (company is null || string.IsNullOrWhiteSpace(company.Email))
        {
            return new CompanyProfileReply { Found = false };
        }

        return new CompanyProfileReply
        {
            Found = true,
            CompanyId = company.Id,
            Email = company.Email
        };
    }
}
