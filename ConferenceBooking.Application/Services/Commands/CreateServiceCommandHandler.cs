using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.ValueObjects;
using Mediator;


namespace ConferenceBooking.Application.Services.Commands;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateServiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = Service.Create(request.Name, Money.Uah(request.Price));

        await _context.Services.AddAsync(service, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}
