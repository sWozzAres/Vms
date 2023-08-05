namespace Vms.Web.Shared;

public record ActivityLogDto(Guid Id, string Text, DateTimeOffset EntryDate, string UserName);
public record AddNoteDto(string Text);