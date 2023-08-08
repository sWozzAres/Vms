namespace Vms.Domain.System
{
    public class TaskLog
    {
        public long Id { get; set; }
        public Guid DocumentId { get; set; }
        public string TaskName { get; set; } = null!;
        public string Log { get; set; } = null!;
        public DateTimeOffset EntryDate { get; set; }
    }
}

namespace Vms.Domain.System.Configuration
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
        }
    }
}