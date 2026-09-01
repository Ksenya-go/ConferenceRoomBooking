using ConferenceBooking.Application.Common.Extensions;
using ConferenceBooking.Application.Common.Interfaces;
using Mediator;

namespace ConferenceBooking.Application.Services.Commands;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteServiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var service = await _context.GetServiceOrThrowAsync(request.ServiceId, cancellationToken);

        service.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
