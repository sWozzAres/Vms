namespace Utopia.Api.Domain.System
{
    public class Follower(Guid documentId, string userId)
    {
        public long Id { get; private set; }
        public Guid DocumentId { get; set; } = documentId;
        public string UserId { get; set; } = userId;
    }
}
namespace Utopia.Api.Domain.System.Configuration
{
    public class FollowerEntityTypeConfiguration : IEntityTypeConfiguration<Follower>
    {
        public void Configure(EntityTypeBuilder<Follower> entity)
        {
            entity.ToTable("Followers", "System");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DocumentId, e.UserId }).IsUnique();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}