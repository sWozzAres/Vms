using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    [Table("NotCompleteReasons")]
    public class NotCompleteReason(string companyCode, string code, string name)
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
    public class NotCompleteReasonEntityTypeConfiguration : IEntityTypeConfiguration<NotCompleteReason>
    {
        public void Configure(EntityTypeBuilder<NotCompleteReason> entity)
        {
            entity.ToTable("NotCompleteReasons");

            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.Code).IsFixedLength();

            entity.HasOne(d => d.Company)
                .WithMany(p => p.NotCompleteReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotCompleteReasons_Companies");
        }
    }
}