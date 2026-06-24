using JobLess.Client.Application.Models;
using MediatR;

namespace JobLess.Client.Application.Clients.Commands.ApplyToJob;

public record ApplyToJobCommand(int ClientId, int AdvertisementId) : IRequest<JobApplicationDto>;
