namespace Utopia.Api.Domain.System
{
    public class TaskLog(Guid documentId, string taskName, string log, DateTimeOffset entryDate, string userId)
    {
        [Key]
        public long Id { get; private set; }

        public Guid DocumentId { get; set; } = documentId;

        [StringLength(50)]
        public string TaskName { get; set; } = taskName;

        [StringLength(4000)]
        public string Log { get; set; } = log;

        public DateTimeOffset EntryDate { get; set; } = entryDate;

        public string UserId { get; set; } = userId;
        public User User { get; private set; } = null!;
    }
}

namespace Utopia.Api.Domain.System.Configuration
{
    public class TaskLogEntityTypeConfiguration : IEntityTypeConfiguration<TaskLog>
    {
        public void Configure(EntityTypeBuilder<TaskLog> entity)
        {
            entity.ToTable("TaskLogs", "System", table =>
            {
                table.HasCheckConstraint("Log record should be formatted as JSON", "ISJSON(log)=1");
            });

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}