namespace Catalog.Api.Domain.System
{
    public enum EntityKind
    {
        None = 0,
        Product = 1,
    }

    public class EntityTag(string entityKey, EntityKind entityKind, string name, string content)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string EntityKey { get; private set; } = entityKey;
        public EntityKind EntityKind { get; private set; } = entityKind;
        public string Name { get; private set; } = name;
        public string Content { get; private set; } = content;
        public void Update(string name, string content) => (Name, Content) = (name, content);
    }
}

namespace Catalog.Api.Domain.System.Configuration
{
    public class EntityTagEntityTypeConfiguration : IEntityTypeConfiguration<EntityTag>
    {
        public void Configure(EntityTypeBuilder<EntityTag> entity)
        {
            entity.ToTable("EntityTags", "System");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Id, e.EntityKind }).IsUnique();
            entity.HasIndex(e => new { e.EntityKey, e.EntityKind }).IsUnique();
            entity.Property(e => e.EntityKey).HasMaxLength(64);
            entity.Property(e => e.Name).HasMaxLength(64);
        }
    }
}