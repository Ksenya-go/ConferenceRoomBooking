using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.ValueObjects;
using Mediator;

namespace ConferenceBooking.Application.Services.Commands;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateServiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _context.GetServiceOrThrowAsync(request.ServiceId, cancellationToken);

        service.Rename(request.Name);
        service.UpdatePrice(Money.Uah(request.Price));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}