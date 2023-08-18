namespace Vms.Domain.Core
{
    [Table("RescheduleReasons")]
    public class RescheduleReason(string companyCode, string code, string name)
    {
        public string CompanyCode { get; set; } = companyCode;
        public Company Company { get; private set; } = null!;

        [StringLength(10)]
        public string Code { get; set; } = code;

        [StringLength(32)]
        public string Name { get; set; } = name;
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class RescheduleReasonEntityTypeConfiguration : IEntityTypeConfiguration<RescheduleReason>
    {
        public void Configure(EntityTypeBuilder<RescheduleReason> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.Code).IsFixedLength();

            entity.HasOne(d => d.Company)
                .WithMany(p => p.RescheduleReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RescheduleReasons_Companies");
        }
    }
}