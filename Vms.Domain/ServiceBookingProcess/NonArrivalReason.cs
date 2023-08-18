using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    [Table("NonArrivalReasons")]
    public class NonArrivalReason(string companyCode, string code, string name)
    {
        public string CompanyCode { get; set; } = companyCode;
        public Company Company { get; private set; } = null!;

        [StringLength(10)]
        public string Code { get; set; } = code;

        [StringLength(32)]
        public string Name { get; set; } = name;
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class NonArrivalReasonEntityTypeConfiguration : IEntityTypeConfiguration<NonArrivalReason>
    {
        public void Configure(EntityTypeBuilder<NonArrivalReason> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.Code).IsFixedLength();

            entity.HasOne(d => d.Company)
                .WithMany(p => p.NonArrivalReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NonArrivalReasons_Companies");
        }
    }
}