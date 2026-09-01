using ConferenceBooking.Application.Services.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Services.Queries.GetAllServices;

public record GetAllServicesQuery(bool ActiveOnly = true) : IRequest<List<ServiceDto>>;