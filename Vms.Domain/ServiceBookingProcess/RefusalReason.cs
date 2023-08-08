using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    public class RefusalReason(string companyCode, string code, string name)
    {
        public const int Code_MaxLength = 10;
        public const int Name_MaxLength = 32;
        public string CompanyCode { get; set; } = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
        public string Code { get; set; } = code ?? throw new ArgumentNullException(nameof(code));
        public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));
        internal Company Company { get; set; } = null!;
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class RefusalReasonEntityTypeConfiguration : IEntityTypeConfiguration<RefusalReason>
    {
        public void Configure(EntityTypeBuilder<RefusalReason> entity)
        {
            entity.ToTable("RefusalReasons");

            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(Company.Code_MaxLength)
                .IsFixedLength();
            entity.Property(e => e.Code)
                .HasMaxLength(RefusalReason.Code_MaxLength)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(RefusalReason.Name_MaxLength)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.RefusalReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefusalReasons_Companies");
        }
    }
}