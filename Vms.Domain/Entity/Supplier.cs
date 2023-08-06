using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vms.Domain.Entity.ServiceBookingEntity;

namespace Vms.Domain.Entity
{
    public partial class Supplier
    {
        public const int Code_MaxLength = 8;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public bool IsIndependent { get; set; }
        //public virtual ICollection<VehicleMake> Franchises { get; set; } = null!;
        public virtual ICollection<SupplierFranchise> Franchises { get; set; } = new List<SupplierFranchise>();
        public virtual ICollection<Network> Networks { get; set; } = new List<Network>();
        public virtual ICollection<NetworkSupplier> NetworkSuppliers { get; set; } = new List<NetworkSupplier>();
        public ICollection<SupplierRefusal> ServiceBookingRefusals { get; set; } = new List<SupplierRefusal>();
        private Supplier() { }
        public Supplier(string code, string name, Address address, bool isIndependent)
        {
            Code = code;
            Name = name;
            Address = new Address(address.Street, address.Locality, address.Town, address.Postcode, address.Location.Copy());
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

namespace Vms.Domain.Entity.Configuration
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
    public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(e => e.Code);
            //builder.HasIndex(e => e.Location, "SPATIAL_Supplier");

            builder.Property(e => e.Code)
                .HasMaxLength(Supplier.Code_MaxLength)
                .IsUnicode(false);

            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.OwnsOne(e => e.Address, ce =>
            {
                ce.Property(x => x.Street)
                    .HasMaxLength(Address.Street_MaxLength)
                    .IsUnicode(false);
                ce.Property(x => x.Locality)
                    .HasMaxLength(Address.Locality_MaxLength)
                    .IsUnicode(false);
                ce.Property(x => x.Town)
                    .HasMaxLength(Address.Town_MaxLength)
                    .IsUnicode(false);
                ce.Property(x => x.Postcode)
                    .HasMaxLength(Address.Postcode_MaxLength)
                    .IsUnicode(false);
            });

            builder.OwnsMany(d => d.Franchises, p =>
            {
                p.ToTable("SupplierFranchises");
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