using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    [Table("RefusalReasons")]
    public class RefusalReason(string companyCode, string code, string name)
    {
        public const int Code_MaxLength = 10;
        public const int Name_MaxLength = 32;

        public string CompanyCode { get; set; } = companyCode;
        public Company Company { get; private set; } = null!;

        [StringLength(Code_MaxLength)]
        public string Code { get; set; } = code;

        [StringLength(Name_MaxLength)]
        public string Name { get; set; } = name;
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class RefusalReasonEntityTypeConfiguration : IEntityTypeConfiguration<RefusalReason>
    {
        public void Configure(EntityTypeBuilder<RefusalReason> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.Code).IsFixedLength();

            entity.HasOne(d => d.Company)
                .WithMany(p => p.RefusalReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefusalReasons_Companies");
        }
    }
}