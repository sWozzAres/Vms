using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public partial class Network
    {
        public string CompanyCode { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public virtual Company CompanyCodeNavigation { get; set; } = null!;

        public virtual ICollection<CustomerNetwork> CustomerNetworks { get; set; } = new List<CustomerNetwork>();

        public virtual ICollection<FleetNetwork> FleetNetworks { get; set; } = new List<FleetNetwork>();

        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public virtual ICollection<NetworkSupplier> NetworkSuppliers { get; set; } = new List<NetworkSupplier>();
        private Network() { }
        public Network(string companyCode, string code, string name)
            => (CompanyCode, Code, Name) = (companyCode, code, name);
    }

    public class NetworkSupplier
    {
        public string CompanyCode { get; private set; } = null!;
        public string NetworkCode { get; private set; } = null!;
        public string SupplierCode { get; private set; } = null!;
        public virtual Network Network { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
        private NetworkSupplier() { }
        public NetworkSupplier(string companyCode, string networkCode, string supplierCode)
            => (CompanyCode, NetworkCode, SupplierCode) = (companyCode, networkCode, supplierCode);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class NetworkEntityTypeConfiguration : IEntityTypeConfiguration<Network>
    {
        public void Configure(EntityTypeBuilder<Network> builder)
        {
            builder.ToTable("Networks");

            builder.HasKey(e => new { e.CompanyCode, e.Code });

            builder.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.CompanyCode)
                .HasMaxLength(Company.Code_MaxLength)
                .IsFixedLength();
            builder.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);

            builder.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Networks)
                //.HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Networks_Companies");

            builder.HasMany(d => d.Suppliers).WithMany(p => p.Networks)
            //.UsingEntity<NetworkSupplier>(x =>
            //{
            //    x.ToTable("NetworkSupplier");
            //    x.HasKey("CompanyCode", "NetworkCode", "SupplierCode");
            //});
            .UsingEntity<NetworkSupplier>(
                //"NetworkSupplier",
                r => r.HasOne(e => e.Supplier).WithMany(e => e.NetworkSuppliers)
                    .HasForeignKey(e => e.SupplierCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NetworkSuppliers_Suppliers"),
                l => l.HasOne(e => e.Network).WithMany(e => e.NetworkSuppliers)
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
