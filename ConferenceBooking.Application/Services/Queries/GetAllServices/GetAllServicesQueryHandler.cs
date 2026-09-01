using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Application.Services.Dtos;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Services.Queries.GetAllServices;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, List<ServiceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllServicesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<ServiceDto>> Handle(GetAllServicesQuery request, CancellationToken 
        cancellationToken)
    {
        var query = _context.Services.AsQueryable();

        if (request.ActiveOnly)
        {
            query = query.Where(s => s.IsActive);
        }

        var services = await query
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return services.Select(ServiceDto.FromEntity).ToList();
    }
}