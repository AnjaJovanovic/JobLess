using JobLess.Client.Application.Models;
using JobLess.Client.Domain.Enums;
using MediatR;

namespace JobLess.Client.Application.Clients.Queries.GetApplicationsByAdvertisements;

public record GetApplicationsByAdvertisementsQuery(
    IReadOnlyList<int> AdvertisementIds,
    JobApplicationStatus? Status = null) : IRequest<IReadOnlyList<CompanyApplicationDto>>;
