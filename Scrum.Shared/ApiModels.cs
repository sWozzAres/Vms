using System.ComponentModel.DataAnnotations;

namespace Scrum.Shared;

public class CreateProductRequest
{
    [StringLength(64)]
    public string Name { get; set; } = null!;
}

public record ProductFullDto(Guid Id, string Name);

public record ProductListDto(Guid Id, string Name);

public class UpdateProductRequest(string name)
{
    [StringLength(64)]
    public string Name { get; set; } = name;
}