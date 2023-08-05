namespace Utopia.Blazor.Application.Shared;

public record ListResult<T>(int TotalCount, List<T> Items);
