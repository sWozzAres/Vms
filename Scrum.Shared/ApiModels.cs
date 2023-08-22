using System.ComponentModel.DataAnnotations;

namespace Scrum.Shared;

public class CreateProductRequest(string name)
{
    [StringLength(64)]
    public string Name { get; set; } = name;
}

public record ProductFullDto(Guid Id, string Name);

public record ProductListDto(Guid Id, string Name);

public record UpdateProductRequest(string Name);