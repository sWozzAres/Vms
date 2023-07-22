namespace Vms.Web.Shared;

public record ActivityLogDto(Guid Id, string Text, DateTime EntryDate);
public record AddNoteDto(string Text);