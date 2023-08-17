namespace Utopia.Api.Domain.System
{
    public class TaskLog(Guid documentId, string taskName, string log, DateTimeOffset entryDate, string userId)
    {
        public long Id { get; private set; }
        public Guid DocumentId { get; set; } = documentId;
        public string TaskName { get; set; } = taskName;
        public string Log { get; set; } = log;
        public DateTimeOffset EntryDate { get; set; } = entryDate;
        public string UserId { get; set; } = userId;
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

            entity.HasKey(e => e.Id);

            entity.Property(e => e.TaskName)
                .HasMaxLength(50);

            entity.Property(e => e.Log)
                .HasMaxLength(4000)
                .IsUnicode();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}