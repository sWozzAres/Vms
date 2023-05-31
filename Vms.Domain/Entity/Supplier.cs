using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Supplier
    {
        public string Code { get; set; } = null!;
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Postcode { get; set; } = null!;
        public Geometry Location { get; set; } = null!;
        public bool IsIndependent { get; set; }
        public virtual ICollection<VehicleMake> Franchises { get; set; } = null!;
        public virtual ICollection<Network> Networks { get; set; } = new List<Network>();
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasAlternateKey(e => e.Code);

            builder.ToTable("Supplier");

            //builder.HasIndex(e => e.Location, "SPATIAL_Supplier");

            builder.Property(e => e.Code)
                .HasMaxLength(8)
                .IsUnicode(false);

            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Postcode)
                .HasMaxLength(9)
                .IsUnicode(false);

            builder.HasMany(d => d.Franchises)
                .WithMany(p => p.SupplierCodes)
                .UsingEntity<Dictionary<string, object>>(
                    "SupplierFranchise",
                    l => l.HasOne<VehicleMake>().WithMany().HasForeignKey("Franchise").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_SupplierFranchise_VehicleMake"),
                    r => r.HasOne<Supplier>().WithMany().HasPrincipalKey(k=>k.Code).HasForeignKey("SupplierCode").HasConstraintName("FK_SupplierFranchise_Supplier"),
                    j =>
                    {
                        j.HasKey("SupplierCode", "Franchise");

                        j.ToTable("SupplierFranchise");

                        j.IndexerProperty<string>("SupplierCode").HasMaxLength(8).IsUnicode(false);

                        j.IndexerProperty<string>("Franchise").HasMaxLength(30).IsUnicode(false);
                    });
        }
    }
}