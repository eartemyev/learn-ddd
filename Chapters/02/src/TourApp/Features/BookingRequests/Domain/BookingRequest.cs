namespace TourApp.Features.BookingRequests.Domain;

public sealed class BookingRequest
{
    public const int MaxSeats = 6;

    public Guid Id { get; }

    public Guid DepartureId { get; }

    public DateTimeOffset DepartureAtUtc { get; }

    public int SeatCount { get; private set; }

    public Money PricePerSeat { get; }

    public BookingStatus Status { get; private set; }

    public Money TotalPrice =>
        PricePerSeat.Multiply(SeatCount);

    private BookingRequest(
        Guid id,
        Guid departureId,
        DateTimeOffset departureAtUtc,
        int seatCount,
        Money pricePerSeat)
    {
        Id = id;
        DepartureId = departureId;
        DepartureAtUtc = departureAtUtc;
        SeatCount = seatCount;
        PricePerSeat = pricePerSeat;
        Status = BookingStatus.Draft;
    }

    // BR-BOOK-001, BR-BOOK-007
    public static BookingRequest Create(
        Guid id,
        Guid departureId,
        DateTimeOffset departureAtUtc,
        int seatCount,
        Money pricePerSeat)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Booking ID is required.", nameof(id));

        if (departureId == Guid.Empty)
            throw new ArgumentException(
                "Departure ID is required.",
                nameof(departureId));

        ArgumentNullException.ThrowIfNull(pricePerSeat);

        ValidateSeatCount(seatCount);

        if (pricePerSeat.Amount <= 0)
        {
            throw new BusinessRuleException(
                "BR-BOOK-007",
                "Цена места должна быть положительной.");
        }

        return new BookingRequest(
            id,
            departureId,
            departureAtUtc,
            seatCount,
            pricePerSeat);
    }

    // BR-BOOK-001, BR-BOOK-002
    public void ChangeSeatCount(int count)
    {
        if (Status != BookingStatus.Draft)
        {
            throw new BusinessRuleException(
                "BR-BOOK-002",
                "Изменить количество мест можно " +
                "только в черновике.");
        }

        ValidateSeatCount(count);

        SeatCount = count;
    }

    // BR-BOOK-003, BR-BOOK-006
    public void Submit(DateTimeOffset nowUtc)
    {
        if (Status != BookingStatus.Draft)
        {
            throw new BusinessRuleException(
                "BR-BOOK-003",
                "Отправить можно только черновик.");
        }

        if (nowUtc >= DepartureAtUtc)
        {
            throw new BusinessRuleException(
                "BR-BOOK-003",
                "Экскурсия уже началась.");
        }

        Status = BookingStatus.Submitted;
    }

    // BR-BOOK-004
    public void Cancel(DateTimeOffset nowUtc)
    {
        // Повторная отмена ничего не меняет.
        if (Status == BookingStatus.Cancelled)
            return;

        if (Status == BookingStatus.Submitted)
        {
            var deadline =
                DepartureAtUtc.AddHours(-24);

            if (nowUtc > deadline)
            {
                throw new BusinessRuleException(
                    "BR-BOOK-004",
                    "Срок отмены заявки истёк.");
            }
        }

        Status = BookingStatus.Cancelled;
    }

    private static void ValidateSeatCount(int count)
    {
        if (count is < 1 or > MaxSeats)
        {
            throw new BusinessRuleException(
                "BR-BOOK-001",
                "Количество мест должно быть от 1 до 6.");
        }
    }
}
