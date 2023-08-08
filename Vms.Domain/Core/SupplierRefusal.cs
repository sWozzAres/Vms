using Vms.Domain.ServiceBookingProcess;

namespace Vms.Domain.Core
{
    public class SupplierRefusal(string supplierCode, string companyCode, string code, string name, Guid serviceBookingId)
    {
        public long Id { get; private set; }
        public string SupplierCode { get; set; } = supplierCode;
        public string CompanyCode { get; set; } = companyCode;
        public string Code { get; set; } = code;
        public string Name { get; set; } = name;
        public Guid ServiceBookingId { get; set; } = serviceBookingId;
        public Supplier Supplier { get; set; } = null!;
    }
}
namespace Vms.Domain.Core.Configuration
{
    public class SupplierRefusalEntityTypeConfiguration : IEntityTypeConfiguration<SupplierRefusal>
    {
        public void Configure(EntityTypeBuilder<SupplierRefusal> entity)
        {
            entity.ToTable("SupplierRefusals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.CompanyCode).HasMaxLength(Company.Code_MaxLength);
            entity.Property(e => e.Code).HasMaxLength(RefusalReason.Code_MaxLength);
            entity.Property(e => e.Name).HasMaxLength(RefusalReason.Name_MaxLength);

            entity.HasOne(e => e.Supplier).WithMany(s => s.ServiceBookingRefusals);
        }
    }
}