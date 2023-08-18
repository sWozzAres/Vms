using Vms.Domain.ServiceBookingProcess;

namespace Vms.Domain.Core
{
    /// <summary>
    /// Stores the reason a supplier refused to book a service booking.
    /// </summary>
    [Table("SupplierRefusals")]
    public class SupplierRefusal(string supplierCode, string companyCode, string code, string name, Guid serviceBookingId)
    {
        [Key]
        public long Id { get; private set; }

        public string SupplierCode { get; set; } = supplierCode;
        public Supplier Supplier { get; private set; } = null!;

        public string CompanyCode { get; set; } = companyCode;

        [StringLength(RefusalReason.Code_MaxLength)]
        public string Code { get; set; } = code;
        public RefusalReason RefusalReason { get; private set; } = null!;

        [StringLength(RefusalReason.Name_MaxLength)]
        public string Name { get; set; } = name;

        public Guid ServiceBookingId { get; set; } = serviceBookingId;
        public ServiceBooking ServiceBooking { get; private set; } = null!;
    }
}
namespace Vms.Domain.Core.Configuration
{
    public class SupplierRefusalEntityTypeConfiguration : IEntityTypeConfiguration<SupplierRefusal>
    {
        public void Configure(EntityTypeBuilder<SupplierRefusal> entity)
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.ServiceBookingRefusals);

            entity.HasOne(e => e.RefusalReason)
                .WithMany()
                .HasForeignKey(e => new { e.CompanyCode, e.Code });

            entity.HasOne(e => e.ServiceBooking)
                .WithMany()
                .HasForeignKey(e => e.ServiceBookingId);
        }
    }
}