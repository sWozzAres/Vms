using System.ComponentModel.DataAnnotations;

namespace Scrum.Api.Domain;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The Product Owner.
    /// </summary>
    public Guid Owner { get; set; } 

    public ICollection<ProductBacklogItem> Items { get; private set; } = new List<ProductBacklogItem>();
}