namespace Utopia.Shared;

public record ActivityLogDto(Guid Id, string Text, DateTimeOffset EntryDate, string UserName);
