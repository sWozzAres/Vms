using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Network: IMultiTenantEntity
    {
        public string CompanyCode { get; set; } = null!;

        public string Code { get; set; } = null!;

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual Company CompanyCodeNavigation { get; set; } = null!;

        public virtual ICollection<CustomerNetwork> CustomerNetworks { get; set; } = new List<CustomerNetwork>();

        public virtual ICollection<FleetNetwork> FleetNetworks { get; set; } = new List<FleetNetwork>();

        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        private Network() { }
        public Network(string companyCode, string code, string name) => (CompanyCode, Code, Name) = (companyCode, code, name);  
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class NetworkEntityTypeConfiguration : IEntityTypeConfiguration<Network>
    {
        public void Configure(EntityTypeBuilder<Network> builder)
        {
            builder.ToTable("Network");

            builder.HasAlternateKey(e => e.Id);
            builder.Property(e => e.Id).UseHiLo("NetworkIds");

            builder.HasKey(e => new { e.CompanyCode, e.Code });

            builder.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);

            builder.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Networks)
                //.HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Network_Company");

            builder.HasMany(d => d.Suppliers).WithMany(p => p.Networks)
                .UsingEntity<Dictionary<string, object>>(
                    "NetworkSupplier",
                    r => r.HasOne<Supplier>().WithMany()
                        .HasForeignKey("SupplierCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NetworkSupplier_Supplier"),
                    l => l.HasOne<Network>().WithMany()
                        .HasForeignKey("CompanyCode", "NetworkCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_NetworkSupplier_Network"),
                    j =>
                    {
                        j.HasKey("CompanyCode", "NetworkCode", "SupplierCode");
                        j.ToTable("NetworkSupplier");
                    });
        }
    }
}