namespace Vms.Web.Shared;

public record ListResult<T>(int TotalCount, List<T> Items);
