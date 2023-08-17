namespace Catalog.Api.Domain;

public class Product(string code, string description)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [StringLength(18)]
    public string Code { get; set; } = code;
    [Required]
    [StringLength(50)]
    public string Description { get; set; } = description;
}
