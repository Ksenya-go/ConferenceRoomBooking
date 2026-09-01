using ConferenceBooking.Application.Services.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Services.Queries.GetServiceById;

public record GetServiceByIdQuery(Guid ServiceId) : IRequest<ServiceDto>;