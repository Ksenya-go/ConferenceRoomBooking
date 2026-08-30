namespace ConferenceBooking.Application.Reports.Dtos;

public record RevenueReportDto(
    DateTime PeriodStart,
    DateTime PeriodEnd,
    decimal TotalRevenue,
    int TotalBookings,
    List<RevenueByRoomDto> ByRoom);

public record RevenueByRoomDto(Guid RoomId, string RoomName, decimal Revenue, int BookingsCount);