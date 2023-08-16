namespace Utopia.Blazor.Application.Shared; 

public record ActivityLogDto(Guid Id, string Text, DateTimeOffset EntryDate, string UserName);
