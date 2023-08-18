namespace Vms.Domain.Core
{
    [Table("Networks")]
    public class Network
    {
        public const int Code_MaxLength = 10;

        public string CompanyCode { get; set; } = null!;
        public Company Company { get; private set; } = null!;

        [StringLength(Code_MaxLength)]
        public string Code { get; set; } = null!;

        [StringLength(32)]
        public string Name { get; set; } = null!;

        public ICollection<CustomerNetwork> CustomerNetworks { get; } = new List<CustomerNetwork>();
        public ICollection<FleetNetwork> FleetNetworks { get; } = new List<FleetNetwork>();
        public ICollection<Supplier> Suppliers { get; } = new List<Supplier>();
        public ICollection<NetworkSupplier> NetworkSuppliers { get; } = new List<NetworkSupplier>();

        private Network() { }
        public Network(string companyCode, string code, string name)
            => (CompanyCode, Code, Name) = (companyCode, code, name);
    }

    public class NetworkSupplier
    {
        public string CompanyCode { get; private set; } = null!;
        public string NetworkCode { get; private set; } = null!;
        public string SupplierCode { get; private set; } = null!;
        public Network Network { get; private set; } = null!;
        public Supplier Supplier { get; private set; } = null!;
        private NetworkSupplier() { }
        public NetworkSupplier(string companyCode, string networkCode, string supplierCode)
            => (CompanyCode, NetworkCode, SupplierCode) = (companyCode, networkCode, supplierCode);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class NetworkEntityTypeConfiguration : IEntityTypeConfiguration<Network>
    {
        public void Configure(EntityTypeBuilder<Network> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.Code });

            builder.Property(e => e.Code).IsFixedLength();

            builder.HasOne(d => d.Company)
                .WithMany(p => p.Networks)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Networks_Companies");

            builder.HasMany(d => d.Suppliers)
                .WithMany(p => p.Networks)
                .UsingEntity<NetworkSupplier>(
                    r => r.HasOne(e => e.Supplier)
                        .WithMany(e => e.NetworkSuppliers)
                        .HasForeignKey(e => e.SupplierCode)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NetworkSuppliers_Suppliers"),
                    l => l.HasOne(e => e.Network)
                        .WithMany(e => e.NetworkSuppliers)
                        .HasForeignKey(e => new { e.CompanyCode, e.NetworkCode })
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NetworkSuppliers_Networks"),
                    j =>
                    {
                        j.HasKey(e => new { e.CompanyCode, e.NetworkCode, e.SupplierCode });
                        j.ToTable("NetworkSuppliers");
                    });
        }
    }
}
