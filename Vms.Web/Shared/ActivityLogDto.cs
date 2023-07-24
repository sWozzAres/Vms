namespace Vms.Web.Shared;

public record ActivityLogDto(Guid Id, string Text, DateTime EntryDate, string UserName);
public record AddNoteDto(string Text);