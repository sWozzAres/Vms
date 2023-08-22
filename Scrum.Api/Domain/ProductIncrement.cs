using System.ComponentModel.DataAnnotations;

namespace Scrum.Api.Domain;

public enum BacklogStatus
{
    None = 0,
    /// <summary>
    /// If it is a Refined Product Backlog.
    /// </summary>
    Ready = 1,
    /// <summary>
    /// The increment is Done or it is not.
    /// </summary>
    Done = 2
}
/// <summary>
/// 1. Every Sprint, the Scrum Team strives to deliver a Product Increment that
///     is Done, usable and potentially releasable.
/// 2. Comprises cohesive PBIs that together at least achieve the Sprint Goal.
/// 3. Creates value along one or more of the Value Streams (end-user / Scrum Team)
/// 4. Reviewed in Sprint Review.
/// 5. Should be deployed as early as possible in order to elicit feedback.
/// 6. Implements whatever requirements are necessary for customers to realize a value 
///     increment in the corresponding Sprint.
/// 7. The Product Backlog details the Product Owner’s Vision for the product as 
///     informed by the expectations of all stakeholders.
/// </summary>
public class ProductIncrement
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid Owner { get; set; }

    /// <summary>
    /// 1. A short statement of the value the Scrum Team intends to create during the Sprint.
    /// 2. Is more important even than the sum of the individual PBIs.
    /// 3. Creates coherence in the PBIs, helping to create a valuable Regular Product Increment.
    /// 4. What the Scrum Team aspires to achieve by the end of the Sprint.
    /// 5. At the end of the Sprint, the team should check the intended value described in the 
    ///     Sprint Goal against reality.
    /// 6, The core of the Regular Product Increment.
    /// </summary>
    public string SprintGoal { get; set; } = null!;

    public DateOnly ExpectedDeliveryDate { get; set; }

    public BacklogStatus Status { get; set; }

    /// <summary>
    /// Collection of Backlog Items that both realise the Sprint Goal and compose this Product Increment.
    /// </summary>
    public ICollection<ProductBacklogItem> BacklogItems { get; } = new List<ProductBacklogItem>();
}
