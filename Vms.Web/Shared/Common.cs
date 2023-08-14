namespace Vms.Web.Shared;

public record ActivityLogDto(Guid Id, string Text, DateTimeOffset EntryDate, string UserName);
public record AddNoteDto(string Text);
public record AddressFullDto(string Street, string Locality, string Town, string Postcode, GeometryFullDto Location);
public record GeometryFullDto(double Latitude, double Longitude);
