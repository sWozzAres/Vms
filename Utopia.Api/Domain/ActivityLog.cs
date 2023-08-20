namespace Utopia.Api.Domain.System
{
    [Table("ActivityLog", Schema = "System")]
    public class ActivityLog(Guid documentId, string text, string userId, string userName,
        DateTimeOffset entryDate, bool isNote)
    {
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid DocumentId { get; private set; } = documentId;

        //[StringLength(4096)]
        public string Text { get; private set; } = text;

        public DateTimeOffset EntryDate { get; private set; } = entryDate;
        public bool IsNote { get; private set; } = isNote;

        public string UserId { get; private set; } = userId;
        public string UserName { get; private set; } = userName;
        public User User { get; private set; } = null!;

    }
}

namespace Utopia.Api.Domain.System.Configuration
{
    public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> entity)
        {
            entity.HasIndex(t => t.DocumentId).IsUnique(false);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}