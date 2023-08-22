using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Scrum.Api.Domain
{

    /// <summary>
    /// 1. An administrative unit that the Product Owner defines, owns, and manages.
    /// 2. Supports one or more ​Value Stream​s: one for the end users, and one for the Scrum Team...
    /// </summary>
    public class Product(string name)
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The Product Owner.
        ///     1. Accountable for product value
        /// </summary>
        public Guid Owner { get; set; }

        [StringLength(64)]
        public string Name { get; set; } = name;

        public ICollection<ProductBacklogItem> BacklogItems { get; } = new List<ProductBacklogItem>();
    }
}
namespace Scrum.Api.Domain.Configuration
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public const string IX_Product_Name = "IX_Products_Name";
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasIndex(e => e.Name).IsUnique();
        }
    }
}