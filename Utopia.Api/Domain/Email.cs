namespace Utopia.Api.Domain.System
{
    public class Email(string recipients, string subject, string body)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Recipients { get; set; } = recipients;
        public string Subject { get; set; } = subject;
        public string Body { get; set; } = body;
    }
}

namespace Utopia.Api.Domain.System.Configuration
{
    public class EmailEntityTypeConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> entity)
        {
            entity.ToTable("Emails", "System");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Recipients).HasMaxLength(256);
            entity.Property(e => e.Subject).HasMaxLength(64);
            entity.Property(e => e.Body).HasMaxLength(2048);
        }
    }
}