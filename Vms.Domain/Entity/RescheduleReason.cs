using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public class RescheduleReason(string companyCode, string code, string name)
    {
        public string CompanyCode { get; set; } = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
        public string Code { get; set; } = code ?? throw new ArgumentNullException(nameof(code));
        public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));
        internal Company Company { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity
{
    public class RescheduleReasonEntityTypeConfiguration : IEntityTypeConfiguration<RescheduleReason>
    {
        public void Configure(EntityTypeBuilder<RescheduleReason> entity)
        {
            entity.ToTable("RescheduleReasons");

            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.RescheduleReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RescheduleReasons_Companies");
        }
    }
}