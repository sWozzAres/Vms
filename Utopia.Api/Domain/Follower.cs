namespace Utopia.Api.Domain.System
{
    [Table("Followers", Schema = "System")]
    public class Follower(Guid documentId, string userId)
    {
        [Key]
        public long Id { get; private set; }

        public Guid DocumentId { get; set; } = documentId;

        public string UserId { get; set; } = userId;
        public User User { get; private set; } = null!;
    }
}
namespace Utopia.Api.Domain.System.Configuration
{
    public class FollowerEntityTypeConfiguration : IEntityTypeConfiguration<Follower>
    {
        public void Configure(EntityTypeBuilder<Follower> entity)
        {
            entity.HasIndex(e => new { e.DocumentId, e.UserId }).IsUnique();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}