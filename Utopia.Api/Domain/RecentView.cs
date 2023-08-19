namespace Utopia.Api.Domain.System
{
    [Table("RecentViews", Schema = "System")]
    public class RecentView(Guid documentId, string userId, DateTime viewDate)
    {
        [Key]
        public long Id { get; private set; }

        public Guid DocumentId { get; set; } = documentId;
        public DateTime ViewDate { get; set; } = viewDate;

        public string UserId { get; set; } = userId;
        public User User { get; private set; } = null!;
    }
}
namespace Utopia.Api.Domain.System.Configuration
{
    public class RecentViewEntityTypeConfiguration : IEntityTypeConfiguration<RecentView>
    {
        public void Configure(EntityTypeBuilder<RecentView> entity)
        {
            entity.HasIndex(e => new { e.DocumentId, e.UserId }).IsUnique();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}