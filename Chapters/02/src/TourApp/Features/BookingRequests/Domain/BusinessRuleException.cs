namespace TourApp.Features.BookingRequests.Domain;

public sealed class BusinessRuleException : Exception
{
    public string RuleId { get; }

    public BusinessRuleException(
        string ruleId,
        string message)
        : base(message)
    {
        RuleId = ruleId;
    }
}