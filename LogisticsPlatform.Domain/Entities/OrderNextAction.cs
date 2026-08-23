using LogisticsPlatform.Domain.Enums;

namespace LogisticsPlatform.Domain.Entities;

public class OrderNextAction
{
    public bool AwaitingClientAction { get; set; }
    public NextActionKind? NextActionKind { get; set; }
    public string? NextActionLabel { get; set; }
    public DateTimeOffset? NextActionDueAt { get; set; }
    public long? NextActionAmountCents { get; set; }
    public string? NextActionDocumentNumber { get; set; }
}
