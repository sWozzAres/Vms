namespace Utopia.Shared;

public record ActivityNotificationDto(long Id, Guid DocumentId, string DocumentKind, string DocumentKey,
    string Text, bool Read, DateTimeOffset EntryDate);