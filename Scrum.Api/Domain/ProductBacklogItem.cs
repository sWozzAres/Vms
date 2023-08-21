using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scrum.Api.Domain
{

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
    /// 1. Represents a set of requirements.
    /// 2. Represents the Enabling Specification for some product update that the
    ///     development team will develop.
    /// 3. Describes deliverables in business (end-user and market) terms.
    /// 4. Describes something that the Development Team​ can develop and deliver to 
    ///     add value to relevant stakeholders when Done.
    /// 5. NOT - user stories, story boards, interaction diagrams, prototypes, user narratives, etc
    ///     a) One PBI may satisfy the requirements germane to several user stories.
    /// 6. Unit of administration, estimation, and delivery
    /// 7. Should focus on what, when and for whom rather than how.
    /// 8. Most PBIs relate to the Regular Product Increment, however a PBI can describe any 
    ///     deliverable that increases stakeholder value.
    /// 9. A PBI can describe anything that has potential value to a stakeholder
    ///     a) market
    ///     b) reduces cost to the enterprise
    ///     c) reduces effort for the development team
    ///     d) a tool that helps the Product Owner tool better do its work
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
        public string ValueDescription { get; set; } = null!;

        /// <summary>
        /// Number of days of work estimated by the Development Team for completion.
        /// </summary>
        public int EstimatedDays { get; set; }

        /// <summary>
        /// 1. Notes about the PBI’s contribution to Value and ROI.
        /// 2. Stakeholder-facing decisions that the Product Owner has taken.
        /// 3. Agreements between the Product Owner and Development Team—the things they 
        ///     should write down to help them together remember.
        /// 4. Development estimates provided by the Development Team that will implement 
        ///     this PBI.
        /// </summary>
        public string Notes { get; set; } = null!;

        public DateOnly DeliveryDate { get; set; }

        /// <summary>
        /// Specifies that the <see cref="DeliveryDate"/> depends on a particular calendar date.
        /// </summary>
        public bool IsFixedDeliveryDate { get; set; }

        /// <summary>
        /// Individual value estimate (High Value First ordering).
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// Roi estimate (ROI ordered backlog).
        /// </summary>
        public int? Roi { get; set; }

        public Guid? ProductIncrementId { get; set; }
        public ProductIncrement? ProductIncrement { get; private set; } = null!;

        public ICollection<ProductBacklogItem> DependsOn { get; } = new List<ProductBacklogItem>();

        public ICollection<SprintBacklogItem> SprintBacklogItems { get; } = new List<SprintBacklogItem>();
    }
}
namespace Scrum.Api.Domain.Configuration
{
    internal class ProductBacklogItemEntityTypeConfiguration : IEntityTypeConfiguration<ProductBacklogItem>
    {
        public void Configure(EntityTypeBuilder<ProductBacklogItem> builder)
        {
            
        }
    }
}