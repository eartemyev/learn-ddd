namespace TourApp.Features.BookingRequests.Domain;

public sealed record Money
{
    public decimal Amount { get; }

    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount));

        if (string.IsNullOrWhiteSpace(currency)
            || currency.Length != 3
            || !currency.All(
                c => c is >= 'A' and <= 'Z'))
        {
            throw new ArgumentException(
                "Currency must contain 3 uppercase letters.",
                nameof(currency));
        }

        Amount = amount;
        Currency = currency;
    }

    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity));

        return new Money(
            Amount * quantity,
            Currency);
    }
}