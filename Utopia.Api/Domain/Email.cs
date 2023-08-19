namespace Utopia.Api.Domain.System
{
    [Table("Emails", Schema = "System")]
    public class Email(string recipients, string subject, string body)
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(256)]
        public string Recipients { get; set; } = recipients;

        [StringLength(64)]
        public string Subject { get; set; } = subject;

        [StringLength(2048)]
        public string Body { get; set; } = body;
    }
}

namespace Utopia.Api.Domain.System.Configuration
{
    public class EmailEntityTypeConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> entity)
        {
        }
    }
}