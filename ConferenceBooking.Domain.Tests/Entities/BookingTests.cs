using FluentAssertions;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Enums;
using ConferenceBooking.Domain.ValueObjects;

namespace ConferenceBooking.Domain.Tests.Entities;

public class BookingTests
{
    // Створення проміжку часу в межах одного дня
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
        var serviceIds = new List<Guid> { Guid.NewGuid() };
        var totalPrice = Money.Uah(3000);

        // Act
        var booking = Booking.Create(roomId, timeRange, serviceIds, totalPrice);

        // Assert
        booking.Id.Should().NotBeEmpty();
        booking.RoomId.Should().Be(roomId);
        booking.TimeRange.Should().Be(timeRange);
        booking.TotalPrice.Should().Be(totalPrice);
        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.SelectedServiceIds.Should().BeEquivalentTo(serviceIds);
    }

    [Fact]
    // Перевірка, що при створенні бронювання встановлюється поточнbq час
    public void Create_SetsCreatedAtUtc_CloseToNow()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Guid>(), Money.Uah(1000));

        var after = DateTime.UtcNow;

        // Assert: CreatedAtUtc має бути між моментом до і після виклику Create
        booking.CreatedAtUtc.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    [Fact]
    public void Create_DuplicateServiceIds_KeepsOnlyDistinctValues()
    {
        // Arrange: клієнт випадково передав ту саму послугу двічі
        var serviceId = Guid.NewGuid();
        var serviceIds = new List<Guid> { serviceId, serviceId };

        // Act
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), serviceIds, Money.Uah(1000));

        // Assert: у результаті послуга залишається одна
        booking.SelectedServiceIds.Should().HaveCount(1);
        booking.SelectedServiceIds.Should().Contain(serviceId);
    }

    [Fact]
    public void Create_NoServices_ReturnsBookingWithEmptyServiceList()
    {
        // Act
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Guid>(), Money.Uah(1000));

        // Assert
        booking.SelectedServiceIds.Should().BeEmpty();
    }

    [Fact]
    public void Cancel_ConfirmedBooking_ChangesStatusToCancelled()
    {
        // Arrange
        var booking = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Guid>(), Money.Uah(1000));

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
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Guid>(), Money.Uah(1000));
        
        booking.Cancel();

        // Act
        var act = () => booking.Cancel();

        // Assert: Не можна скасувати вже скасоване бронювання
        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(10, 12, 11, 13, true)]   // перетинаються
    [InlineData(10, 12, 12, 14, false)]  // стикуються впритул — не перетин
    [InlineData(10, 12, 14, 16, false)]  // немає перетину взагалі
    public void OverlapsWith_VariousTimeRanges_ReturnsExpectedResult(
        int bookingStart, int bookingEnd, int otherStart, int otherEnd, bool expectedOverlap)
    {
        // Arrange
        var booking = Booking.Create(
            Guid.NewGuid(),
            CreateTimeRange(bookingStart, bookingEnd),
            Enumerable.Empty<Guid>(),
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
            Guid.NewGuid(),
            CreateTimeRange(),
            Enumerable.Empty<Guid>(),
            Money.Uah(1000));

        // Act: перевірка, чи об'єкт вважається рівним самому собі
        var result = booking.Equals(booking);

        // Assert
        result.Should().BeTrue();
    }


    [Fact]
    public void TwoDifferentBookings_AreNotEqual()
    {
        // Arrange
        var booking1 = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Guid>(), Money.Uah(1000));
        var booking2 = Booking.Create(
            Guid.NewGuid(), CreateTimeRange(), Enumerable.Empty<Guid>(), Money.Uah(1000));

        // Act
        var result = booking1.Equals(booking2);

        // Assert
        result.Should().BeFalse();
        booking1.Should().NotBe(booking2);
    }
}