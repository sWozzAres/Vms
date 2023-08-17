namespace Utopia.Api.Domain.System
{
    public class RecentView(Guid documentId, string userId, DateTime viewDate)
    {
        public long Id { get; private set; }
        public Guid DocumentId { get; set; } = documentId;
        public string UserId { get; set; } = userId;
        public DateTime ViewDate { get; set; } = viewDate;
    }
}
namespace Utopia.Api.Domain.System.Configuration
{
    public class RecentViewEntityTypeConfiguration : IEntityTypeConfiguration<RecentView>
    {
        public void Configure(EntityTypeBuilder<RecentView> entity)
        {
            entity.ToTable("RecentViews", "System");

            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DocumentId, e.UserId }).IsUnique();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}