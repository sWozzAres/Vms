using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public class ActivityLog(Guid documentId, string text)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid DocumentId { get; private set; } = documentId;
        public string Text { get; private set; } = text ?? throw new ArgumentNullException(nameof(text));
        public DateTime EntryDate { get; private set; } = DateTime.Now;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> entity)
        {
            entity.ToTable("ActivityLog");

            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.DocumentId).IsUnique(false);

            entity.Property(t => t.Text).HasMaxLength(1024).IsRequired();
        }
    }
}