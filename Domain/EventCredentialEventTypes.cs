namespace Domain;

public static class EventCredentialEventTypes
{
    public const string Siege = "Asedio";
    public const string EpicRaidBoss = "Raid boss epico";
    public const string RegroupRaidBoss = "Raid boss regroup";

    public static readonly TimeSpan BorrowedCredentialsVisibilityWindow = TimeSpan.FromHours(2);

    public static bool RevealsBorrowedCharacterCredentials(string? eventType) =>
        string.Equals(eventType, Siege, StringComparison.Ordinal)
        || string.Equals(eventType, EpicRaidBoss, StringComparison.Ordinal)
        || string.Equals(eventType, RegroupRaidBoss, StringComparison.Ordinal);

    public static bool IsWithinBorrowedCredentialsWindow(DateTime eventTime, DateTime? referenceUtc = null)
    {
        var now = referenceUtc ?? DateTime.UtcNow;
        var offset = now - ToUtc(eventTime);
        return offset >= -BorrowedCredentialsVisibilityWindow && offset <= BorrowedCredentialsVisibilityWindow;
    }

    public static bool IsBeforeBorrowedCredentialsWindow(DateTime eventTime, DateTime? referenceUtc = null)
    {
        var now = referenceUtc ?? DateTime.UtcNow;
        return ToUtc(eventTime) - now > BorrowedCredentialsVisibilityWindow;
    }

    public static bool IsAfterBorrowedCredentialsWindow(DateTime eventTime, DateTime? referenceUtc = null)
    {
        var now = referenceUtc ?? DateTime.UtcNow;
        return now - ToUtc(eventTime) > BorrowedCredentialsVisibilityWindow;
    }

    private static DateTime ToUtc(DateTime eventTime) =>
        eventTime.Kind switch
        {
            DateTimeKind.Utc => eventTime,
            DateTimeKind.Local => eventTime.ToUniversalTime(),
            _ => DateTime.SpecifyKind(eventTime, DateTimeKind.Utc),
        };
}
