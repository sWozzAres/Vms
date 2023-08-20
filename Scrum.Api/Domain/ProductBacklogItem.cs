using System.ComponentModel.DataAnnotations;

namespace Scrum.Api.Domain;

public enum PbiStatus
{
    None = 0,
    Ready = 1,
    /// <summary>
    /// Delivery-ready
    /// </summary>
    Done = 2,
}

/// <summary>
/// Represents a set of requirements.
/// </summary>
public class ProductBacklogItem
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public PbiStatus Status { get; set; }

    /// <summary>
    /// Helps relevant stakeholders crisply recall what the deliverable encompasses.
    /// </summary>
    [StringLength(64)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Describes something that the ​Development Team​ can develop and deliver to add value 
    /// to relevant stakeholders when Done.
    /// </summary>
    [StringLength(4096)]
    public string Description { get; set; } = null!;

    /// <summary>
    /// Number of days of work estimated by the Development Team for completion.
    /// </summary>
    public int EstimatedDays { get; set; }

    /// <summary>
    /// Notes about the PBI’s contribution to Value and ROI.
    /// </summary>
    public string Notes { get; set; } = null!;

    public DateOnly DeliveryDate { get; set; }

    /// <summary>
    /// Specifies that the <see cref="DeliveryDate"/> depends on a particular calendar date.
    /// </summary>
    public bool IsFixedDate { get; set; }

    /// <summary>
    /// Individual value estimate (High Value First ordering).
    /// </summary>
    public int? Value { get; set; }

    /// <summary>
    /// Roi estimate (ROI ordered backlog).
    /// </summary>
    public int? Roi { get; set; }

    public Guid? ProductIncrementId { get; set; }
    public ProductIncrement ProductIncrement { get; private set; } = null!;
}
