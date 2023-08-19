using System.Reflection.Metadata;

namespace Utopia.Api.Domain.System
{
    [Table("ActivityNotifications", Schema = "System")]
    public class ActivityNotification(Guid documentId, string documentKind, string documentKey, string userId, Guid activityLogId, DateTime entryDate)
    {
        [Key]
        public long Id { get; set; }

        public Guid DocumentId { get; private set; } = documentId;
        [StringLength(32)]
        public string DocumentKind { get; set; } = documentKind;
        [StringLength(16)]
        public string DocumentKey { get; set; } = documentKey;
        public DateTime EntryDate { get; set; } = entryDate;
        public bool Read { get; private set; }

        public Guid ActivityLogId { get; set; } = activityLogId;
        public ActivityLog ActivityLog { get; private set; } = null!;

        public string UserId { get; set; } = userId;
        public User User { get; private set; } = null!;
        public void MarkAsRead() => Read = true;
    }
}
namespace Utopia.Api.Domain.System.Configuration
{
    public class ActivityNotificationEntityTypeConfiguration : IEntityTypeConfiguration<ActivityNotification>
    {
        public void Configure(EntityTypeBuilder<ActivityNotification> builder)
        {
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(e => new { e.UserId, e.EntryDate })
                .IsDescending(false, true);

            builder.HasOne(e => e.ActivityLog)
                .WithMany()
                .HasForeignKey(e => e.ActivityLogId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}