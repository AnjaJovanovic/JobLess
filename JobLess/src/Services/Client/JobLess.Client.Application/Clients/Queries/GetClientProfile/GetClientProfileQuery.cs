using JobLess.Client.Application.Models;
using MediatR;

namespace JobLess.Client.Application.Clients.Queries.GetClientProfile;

public record GetClientProfileQuery(int ClientId) : IRequest<ClientProfileDto?>;
