namespace LogisticsPlatform.Domain.Orders;

public static class OrderNumberRules
{
    public static bool IsDraftNumber(string? number) =>
        !string.IsNullOrWhiteSpace(number) &&
        number.TrimStart().StartsWith("DRAFT-", StringComparison.OrdinalIgnoreCase);

    public static string Normalize(string number) => number.Trim();
}
