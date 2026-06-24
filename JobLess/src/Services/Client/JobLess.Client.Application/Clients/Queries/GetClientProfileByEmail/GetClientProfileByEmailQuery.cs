using JobLess.Client.Application.Models;
using MediatR;

namespace JobLess.Client.Application.Clients.Queries.GetClientProfileByEmail;

public record GetClientProfileByEmailQuery(string Email) : IRequest<ClientProfileDto?>;
