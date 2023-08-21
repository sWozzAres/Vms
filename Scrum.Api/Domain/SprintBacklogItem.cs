using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace Scrum.Api.Domain
{
    public class SprintBacklogItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductBacklogItemId { get; set; }
        public ProductBacklogItem ProductBacklogItem { get; private set; } = null!;

        public bool IsDone { get; set; }

        /// <summary>
        /// Reminds the Development Team of the outcome of accumulated work plan discussions to date.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Certain SBIs depend on the completion of others.
        /// </summary>
        public ICollection<SprintBacklogItem> DependsOn { get; } = new List<SprintBacklogItem>();
    }
}
