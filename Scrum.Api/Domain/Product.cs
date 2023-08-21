using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace Scrum.Api.Domain;

/// <summary>
/// 1. An administrative unit that the Product Owner defines, owns, and manages.
/// 2. Supports one or more ​Value Stream​s: one for the end users, and one for the Scrum Team...
/// </summary>
public class Product
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The Product Owner.
    ///     1. Accountable for product value
    /// </summary>
    public Guid Owner { get; set; }

    public ICollection<ProductBacklogItem> BacklogItems { get; } = new List<ProductBacklogItem>();
}