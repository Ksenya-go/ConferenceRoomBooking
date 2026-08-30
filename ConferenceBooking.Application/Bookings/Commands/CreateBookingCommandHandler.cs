using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Common.Exceptions;
using ConferenceBooking.Application.Common.Interfaces;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Services;
using ConferenceBooking.Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Application.Bookings.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingResult>
{
    private readonly IApplicationDbContext _context;
    private readonly PricingService _pricingService;
    private readonly IBookingTransactionGuard _bookingGuard;

    public CreateBookingCommandHandler(
        IApplicationDbContext context,
        PricingService pricingService,
        IBookingTransactionGuard bookingGuard)
    {
        _context = context;
        _pricingService = pricingService;
        _bookingGuard = bookingGuard;
    }

    public async ValueTask<CreateBookingResult> Handle(
        CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Виконується все в одній транзакції з блокуванням рядків, щоб уникнути проблем
        // з конкурентним доступом
        return await _bookingGuard.ExecuteAsync(async ct =>
        {
            var room = await _context.Rooms
                .Include(r => r.Services)
                .FirstOrDefaultAsync(r => r.Id == request.RoomId, ct);

            if (room is null)
            {
                throw new NotFoundException(RoomErrorMessages.RoomNotFound(request.RoomId));
            }

            if (!room.IsActive)
            {
                throw new BusinessRuleException(BookingErrorMessages.RoomInactive(room.Name));
            }
                
            var timeRange = TimeRange.Create(request.Start, request.End);

            var allowedServiceIds = room.Services.Select(rs => rs.ServiceId).ToHashSet();
            var invalidServiceIds = request.SelectedServiceIds.Where(id => !allowedServiceIds.Contains(id)).ToList();
            if (invalidServiceIds.Count != 0)
            {
                throw new BusinessRuleException(
                                   BookingErrorMessages.ServicesNotAvailable(room.Name, invalidServiceIds));
            }

            var overlapping = await _context.Bookings
                .Where(b => b.RoomId == request.RoomId)
                .Where(b => b.TimeRange.Start < request.End && request.Start < b.TimeRange.End)
                .AnyAsync(ct);

            if (overlapping)
            {
                throw new BusinessRuleException(BookingErrorMessages.RoomAlreadyBooked);
            }

            var selectedServices = await _context.Services
                .Where(s => request.SelectedServiceIds.Contains(s.Id))
                .ToListAsync(ct);

            var totalPrice = _pricingService.CalculateTotalPrice(room, timeRange, selectedServices);

            var booking = Booking.Create(room.Id, timeRange, request.SelectedServiceIds, totalPrice);

            await _context.Bookings.AddAsync(booking, ct);
            await _context.SaveChangesAsync(ct);

            return new CreateBookingResult(booking.Id, totalPrice.Amount);
        }, cancellationToken);
    }
}