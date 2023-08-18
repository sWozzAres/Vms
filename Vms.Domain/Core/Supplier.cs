namespace Vms.Domain.Core
{
    [Table("Suppliers")]
    public class Supplier
    {
        public const int Code_MaxLength = 8;

        public Guid Id { get; private set; }

        [Key]
        [StringLength(Code_MaxLength)]
        public string Code { get; set; } = null!;

        [StringLength(50)]
        public string Name { get; set; } = null!;

        public Address Address { get; internal set; } = null!;

        public bool IsIndependent { get; set; }

        public ICollection<SupplierFranchise> Franchises { get; } = new List<SupplierFranchise>();
        public ICollection<Network> Networks { get; } = new List<Network>();
        public ICollection<NetworkSupplier> NetworkSuppliers { get; } = new List<NetworkSupplier>();
        public ICollection<SupplierRefusal> ServiceBookingRefusals { get; } = new List<SupplierRefusal>();

        private Supplier() { }
        public Supplier(string code, string name, Address address, bool isIndependent)
        {
            Id = Guid.NewGuid();
            Code = code.ToUpper();
            Name = name;
            Address = new Address(address);
            IsIndependent = isIndependent;
        }
    }

    public class SupplierFranchise
    {
        public string SupplierCode { get; set; } = null!;
        public string Franchise { get; set; } = null!;
        public Supplier Supplier { get; private set; } = null!;
        public VehicleMake Make { get; private set; } = null!;
        private SupplierFranchise() { }
        public SupplierFranchise(string supplierCode, string franchise)
            => (SupplierCode, Franchise) = (supplierCode, franchise);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(e => e.Code);
            //builder.HasIndex(e => e.Location, "SPATIAL_Supplier");

            builder.Property(e => e.Id)
                .HasDefaultValueSql("newid()");

            builder.Property(e => e.Code).IsFixedLength();

            builder.OwnsOne(e => e.Address);

            builder.OwnsMany(d => d.Franchises, p =>
            {
                p.ToTable("SupplierFranchises");
                p.HasKey(p => new { p.SupplierCode, p.Franchise });

                p.WithOwner(p => p.Supplier)
                    .HasForeignKey(p => p.SupplierCode);

                p.HasOne(p => p.Make)
                    .WithMany()
                    .HasForeignKey(p => p.Franchise);
            });
        }
    }
}