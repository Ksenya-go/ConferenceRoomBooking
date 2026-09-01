using ConferenceBooking.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ConferenceBooking.Domain.Tests.ValueObjects;

public class TimeRangeTests
{
    [Fact]
    public void Create_EndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2026, 9, 1, 12, 0, 0);
        var end = new DateTime(2026, 9, 1, 10, 0, 0);

        // Act
        var act = () => TimeRange.Create(start, end);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_EndEqualsStart_ThrowsArgumentException()
    {
        // Arrange
        var moment = new DateTime(2026, 9, 1, 10, 0, 0);

        // Act
        var act = () => TimeRange.Create(moment, moment);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_DifferentDays_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2026, 9, 1, 22, 0, 0);
        var end = new DateTime(2026, 9, 2, 2, 0, 0);

        // Act
        var act = () => TimeRange.Create(start, end);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Duration_TwoHourRange_ReturnsTwoHours()
    {
        // Arrange
        var range = TimeRange.Create(new DateTime(2026, 9, 1, 10, 0, 0),
            new DateTime(2026, 9, 1, 12, 0, 0));

        // Act
        var duration = range.Duration;

        // Assert
        duration.Should().Be(TimeSpan.FromHours(2));
    }

    [Theory]
    [InlineData(10, 12, 11, 13, true)]
    [InlineData(10, 12, 12, 14, false)]
    [InlineData(10, 12, 14, 16, false)]
    [InlineData(10, 14, 11, 12, true)]
    public void Overlaps_VariousRanges_ReturnsExpectedResult(int start1,int end1,
        int start2,int end2,bool expected)
    {
        // Arrange
        var date = new DateTime(2026, 9, 1);

        var range1 = TimeRange.Create(date.AddHours(start1), date.AddHours(end1));

        var range2 = TimeRange.Create(date.AddHours(start2),date.AddHours(end2));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Equals_SameStartAndEnd_ReturnsTrue()
    {
        // Arrange
        var date = new DateTime(2026, 9, 1);

        var range1 = TimeRange.Create(date.AddHours(10),date.AddHours(12));

        var range2 = TimeRange.Create(date.AddHours(10),date.AddHours(12));

        // Act
        var result = range1.Equals(range2);

        // Assert
        result.Should().BeTrue();
    }
}