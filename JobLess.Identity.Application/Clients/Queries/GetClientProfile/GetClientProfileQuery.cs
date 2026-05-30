using JobLess.Identity.Application.Models;
using MediatR;

namespace JobLess.Identity.Application.Clients.Queries.GetClientProfile;

public record GetClientProfileQuery(int ClientId) : IRequest<ClientProfileDto?>;
