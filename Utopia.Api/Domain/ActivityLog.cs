namespace Utopia.Api.Domain.System
{
    public class ActivityLog(Guid documentId, string text, string userId, string userName, 
        DateTimeOffset entryDate, bool isNote)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        //public string CompanyCode { get; set; } = companyCode;
        public Guid DocumentId { get; private set; } = documentId;
        public string Text { get; private set; } = text ?? throw new ArgumentNullException(nameof(text));
        public DateTimeOffset EntryDate { get; private set; } = entryDate;
        public string UserName { get; private set; } = userName;
        public string UserId { get; private set; } = userId;
        public bool IsNote { get; private set; } = isNote;
    }
}

namespace Utopia.Api.Domain.System.Configuration
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