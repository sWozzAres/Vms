namespace Catalog.Shared;

public class ProductFullDto(Guid id, string code, string description, bool isFollowing)
{
    public Guid Id { get; set; } = id;
    public string Code { get; set; } = code;
    public string Description { get; set; } = description;
    public bool IsFollowing { get; set; } = isFollowing;
    public ProductDto ToDto() =>
        new(Id, Code, Description);
}

public class ProductDto(Guid id, string code, string description) : ICopyable<ProductDto>
{
    public Guid Id { get; set; } = id;
    [Required]
    [StringLength(18)]
    public string Code { get; set; } = code;
    [Required]
    [StringLength(50)]
    public string Description { get; set; } = description;

    public void CopyFrom(ProductDto source)
    {
        Id = source.Id;
        Code = source.Code;
        Description = source.Description;
    }
}

public record ProductListDto(Guid Id, string Code, string Name);
public enum ProductListOptions
{
    All = 0,
    Recent = 1,
    Following = 2,
}