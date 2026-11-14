using TourApp.Features.BookingRequests.Domain;

public class BookingRequestTests
{
    private static readonly DateTimeOffset DepartureAt =
        new(2027, 1, 20, 12, 0, 0, TimeSpan.Zero);

    private static BookingRequest CreateBooking(int seats = 2)
    {
        return BookingRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DepartureAt,
            seats,
            new Money(1500m, "RUB"));
    }

    // BR-BOOK-001
    [Theory]
    [Trait("Requirement", "BR-BOOK-001")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(7)]
    public void Create_InvalidSeatCount_Throws(int seats)
    {
        var exception =
            Assert.Throws<BusinessRuleException>(
                () => CreateBooking(seats));

        Assert.Equal(
            "BR-BOOK-001",
            exception.RuleId);
    }

    // BR-BOOK-002
    [Fact]
    [Trait("Requirement", "BR-BOOK-002")]
    public void ChangeSeats_AfterSubmit_Throws()
    {
        var booking = CreateBooking();

        booking.Submit(DepartureAt.AddDays(-5));

        Assert.Throws<BusinessRuleException>(
            () => booking.ChangeSeatCount(3));
    }

    // BR-BOOK-003
    [Fact]
    [Trait("Requirement", "BR-BOOK-003")]
    public void Submit_AfterDeparture_Throws()
    {
        var booking = CreateBooking();

        Assert.Throws<BusinessRuleException>(
            () => booking.Submit(
                DepartureAt.AddMinutes(1)));
    }

    // BR-BOOK-004
    [Theory]
    [Trait("Requirement", "BR-BOOK-004")]
    [InlineData(25, true)]
    [InlineData(24, true)]
    [InlineData(23, false)]
    public void Cancel_DependsOnDeadline(
        int hoursBefore,
        bool shouldSucceed)
    {
        var booking = CreateBooking();

        booking.Submit(DepartureAt.AddDays(-5));

        var now = DepartureAt.AddHours(-hoursBefore);

        if (shouldSucceed)
        {
            booking.Cancel(now);

            Assert.Equal(
                BookingStatus.Cancelled,
                booking.Status);
        }
        else
        {
            Assert.Throws<BusinessRuleException>(
                () => booking.Cancel(now));

            Assert.Equal(
                BookingStatus.Submitted,
                booking.Status);
        }
    }

    // BR-BOOK-005
    [Fact]
    [Trait("Requirement", "BR-BOOK-005")]
    public void TotalPrice_DependsOnSeatCount()
    {
        var booking = CreateBooking(3);

        Assert.Equal(
            new Money(4500m, "RUB"),
            booking.TotalPrice);
    }

    // BR-BOOK-006
    [Fact]
    [Trait("Requirement", "BR-BOOK-006")]
    public void CancelledBooking_CannotBeSubmitted()
    {
        var booking = CreateBooking();

        booking.Cancel(DepartureAt.AddDays(-5));

        Assert.Throws<BusinessRuleException>(
            () => booking.Submit(
                DepartureAt.AddDays(-4)));
    }
}


