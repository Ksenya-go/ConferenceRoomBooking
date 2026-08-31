using FluentAssertions;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Tests.Entities;

public class BookingTests
{
    private static TimeRange CreateTimeRange(int startHour = 10, int endHour = 12)
    {
        var date = new DateTime(2026, 9, 1);
        return TimeRange.Create(date.AddHours(startHour), date.AddHours(endHour));
    }

    [Fact]
    public void Create_ValidData_ReturnsConfirmedBooking()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var timeRange = CreateTimeRange();
        var wifi = Service.Create("Wi-Fi", Money.Uah(300));
        var totalPrice = Money.Uah(3000);

        // Act
        var booking = Booking.Create(roomId, timeRange, new[] { wifi }, totalPrice);
        
        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.RoomId.Should().Be(roomId);
        booking.TimeRange.Should().Be(timeRange);
        booking.TotalPrice.Should().Be(totalPrice);
        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.SelectedServiceIds.Should().BeEquivalentTo(new[] { wifi.Id });
    }

    [Fact]
    public void Create_SetsCreatedAtUtc_CloseToNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        
        // Act
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));

        var after = DateTime.UtcNow;
        
        // Assert
        booking.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_DuplicateServices_KeepsOnlyDistinctValues()
    {
        // Arrange
        var wifi = Service.Create("Wi-Fi", Money.Uah(300));
        var services = new[] { wifi, wifi };
        // Act
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), services, Money.Uah(1000));
        // Assert
        booking.Services.Should().HaveCount(1);
        booking.SelectedServiceIds.Should().Contain(wifi.Id);
    }

    [Fact]
    public void Create_NoServices_ReturnsBookingWithEmptyServiceList()
    {
        // Act
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));
        // Assert
        booking.Services.Should().BeEmpty();
        booking.SelectedServiceIds.Should().BeEmpty();
    }

    [Fact]
    // Зберігається ціна послуги такою, якою вона була на момент бронювання
    public void Create_ServicePriceChangesLater_BookingKeepsOriginalPriceSnapshot()
    {
        // Arrange
        var wifi = Service.Create("Wi-Fi", Money.Uah(300));

        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), new[] { wifi }, Money.Uah(2300));

        // Act
        // Ціну в довіднику змінюють вже після бронювання
        wifi.UpdatePrice(Money.Uah(999));
        var bookedService = booking.Services.Single();
        // Assert
        bookedService.ServiceId.Should().Be(wifi.Id);
        bookedService.ServiceName.Should().Be(wifi.Name);
        bookedService.PriceAtBooking.Should().Be(Money.Uah(300));
    }

    [Fact]
    public void Cancel_ConfirmedBooking_ChangesStatusToCancelled()
    {
        // Arrange
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));
        // Act
        booking.Cancel();
        // Assert
        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AlreadyCancelledBooking_ThrowsInvalidOperationException()
    {
        // Arrange
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));
        // Act
        booking.Cancel();

        var act = () => booking.Cancel();
        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(10, 12, 11, 13, true)]
    [InlineData(10, 12, 12, 14, false)]
    [InlineData(10, 12, 14, 16, false)]
    public void OverlapsWith_VariousTimeRanges_ReturnsExpectedResult(
        int bookingStart, int bookingEnd, int otherStart, int otherEnd, bool expectedOverlap)
    {
        // Arrange
        var booking = Booking.Create(
            Guid.NewGuid(),
            CreateTimeRange(bookingStart, bookingEnd),
            Enumerable.Empty<Service>(),
            Money.Uah(1000));

        var otherRange = CreateTimeRange(otherStart, otherEnd);
        // Act
        var result = booking.OverlapsWith(otherRange);
        // Assert
        result.Should().Be(expectedOverlap);
    }

    [Fact]
    public void TwoBookings_WithSameId_AreEqual()
    {
        // Arrange
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));
        // Act
        var result = booking.Equals(booking);
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TwoDifferentBookings_AreNotEqual()
    {
        // Arrange
        var booking1 = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));
        var booking2 = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Service>(), Money.Uah(1000));
        // Act
        var result = booking1.Equals(booking2);
        // Assert
        result.Should().BeFalse();
        booking1.Should().NotBe(booking2);
    }
}