using System.ComponentModel.DataAnnotations;

namespace Scrum.Api.Domain;

public class ProductIncrement
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public ICollection<ProductBacklogItem> Items { get; private set; } = new List<ProductBacklogItem>();
}
