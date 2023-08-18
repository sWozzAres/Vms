using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Api.Domain
{
    [Table("Products")]
    public class Product(string code, string description)
    {
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();
        [StringLength(18)]
        public string Code { get; set; } = code;
        [StringLength(50)]
        public string Description { get; set; } = description;
    }
}
namespace Catalog.Api.Domain.Configuration
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasIndex(e => e.Code).IsUnique();
        }
    }
}