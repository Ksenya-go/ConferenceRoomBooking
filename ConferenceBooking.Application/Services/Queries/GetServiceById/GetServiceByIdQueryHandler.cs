using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Services.Dtos;
using Mediator;

namespace ConferenceBooking.Application.Services.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto>
{
    private readonly IApplicationDbContext _context;

    public GetServiceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ServiceDto> Handle(GetServiceByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var service = await _context.GetServiceOrThrowAsync(request.ServiceId, cancellationToken);

        return ServiceDto.FromEntity(service);
    }
}