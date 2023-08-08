namespace Vms.Domain.System
{
    public class ActivityLog(Guid documentId, string text, string userId, string userName, DateTimeOffset entryDate)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        //public string CompanyCode { get; set; } = companyCode;
        public Guid DocumentId { get; private set; } = documentId;
        public string Text { get; private set; } = text ?? throw new ArgumentNullException(nameof(text));
        public DateTimeOffset EntryDate { get; private set; } = entryDate;
        public string UserName { get; set; } = userName;
        public string UserId { get; set; } = userId;
    }
}

namespace Vms.Domain.System.Configuration
{
    public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> entity)
        {
            entity.ToTable("ActivityLog", "System");

            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.DocumentId).IsUnique(false);

            entity.Property(t => t.Text).HasMaxLength(1024).IsRequired();
            //entity.Property(t => t.CompanyCode).HasMaxLength(Company.Code_MaxLength);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}