using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Supplier
    {
        public string Code { get; set; } = null!;
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public bool IsIndependent { get; set; }
        //public virtual ICollection<VehicleMake> Franchises { get; set; } = null!;
        public virtual ICollection<SupplierFranchise> Franchises { get; set; } = new List<SupplierFranchise>();
        public virtual ICollection<Network> Networks { get; set; } = new List<Network>();
        public virtual ICollection<NetworkSupplier> NetworkSuppliers { get; set; } = new List<NetworkSupplier>();
        private Supplier() { }
        public Supplier(string code, string name, Address address, bool isIndependent)
        {
            Code = code;
            Name = name;
            Address = new Address(address.Street, address.Locality, address.Town, address.Postcode, address.Location);
            IsIndependent = isIndependent;
        }
    }

    public class SupplierFranchise
    {
        public string SupplierCode { get; set; } = null!;
        public string Franchise { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
        public VehicleMake Make { get; set; } = null!;
        private SupplierFranchise() { }
        public SupplierFranchise(string supplierCode, string franchise) => (SupplierCode, Franchise) = (supplierCode, franchise);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.Property(e => e.Id).UseHiLo("SupplierIds");
            builder.HasKey(e => e.Code);

            builder.ToTable("Supplier");

            //builder.HasIndex(e => e.Location, "SPATIAL_Supplier");

            builder.Property(e => e.Code)
                .HasMaxLength(8)
                .IsUnicode(false);

            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.OwnsOne(e => e.Address, ce =>
            {
                ce.Property(x => x.Postcode)
                    .HasMaxLength(9)
                    .IsUnicode(false);
            });

            builder.OwnsMany(d => d.Franchises, p =>
            {
                p.ToTable("SupplierFranchise");
                p.HasKey(p => new { p.SupplierCode, p.Franchise });

                p.WithOwner(p => p.Supplier)
                    .HasForeignKey(p => p.SupplierCode);

                p.HasOne(p => p.Make).WithMany().HasForeignKey(p => p.Franchise);
            });

            //builder.OwnsMany(d => d.Franchises)
            //    .WithOne(p => p.SupplierCodes)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "SupplierFranchise",
            //        l => l.HasOne<VehicleMake>().WithMany().HasForeignKey("Franchise").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SupplierFranchise_VehicleMake"),
            //        r => r.HasOne<Supplier>().WithMany().HasForeignKey("SupplierCode").HasConstraintName("FK_SupplierFranchise_Supplier"),
            //        j =>
            //        {
            //            j.HasKey("SupplierCode", "Franchise");

            //            j.ToTable("SupplierFranchise");

            //            j.IndexerProperty<string>("SupplierCode").HasMaxLength(8).IsUnicode(false);

            //            j.IndexerProperty<string>("Franchise").HasMaxLength(30).IsUnicode(false);
            //        });
        }
    }
}