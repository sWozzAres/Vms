using Vms.Domain.Core;

namespace Vms.Domain.System
{
    public enum EntityKind
    {
        None = 0,
        Company = 1,
        Fleet = 2,
        Vehicle = 3,
        Customer = 4,
        Network = 5,
        Supplier = 6,
        ServiceBooking = 7,
        Driver = 8
    }

    [Table("EntityTags", Schema = "System")]
    public class EntityTag(string? companyCode, string entityKey, EntityKind entityKind, string name, string content)
    {
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string? CompanyCode { get; private set; } = companyCode;

        [StringLength(64)]
        public string EntityKey { get; private set; } = entityKey;

        public EntityKind EntityKind { get; private set; } = entityKind;

        [StringLength(64)]
        public string Name { get; private set; } = name;

        public string Content { get; private set; } = content;

        public void Update(string name, string content) => (Name, Content) = (name, content);
    }
}

namespace Vms.Domain.System.Configuration
{
    public class EntityTagEntityTypeConfiguration : IEntityTypeConfiguration<EntityTag>
    {
        public void Configure(EntityTypeBuilder<EntityTag> entity)
        {
            entity.HasIndex(e => new { e.CompanyCode, e.Id, e.EntityKind }).IsUnique();
            entity.HasIndex(e => new { e.EntityKey, e.EntityKind }).IsUnique();

            entity.HasOne<Company>()
                .WithMany()
                .HasForeignKey(e => e.CompanyCode);
        }
    }
}