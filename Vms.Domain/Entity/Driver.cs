using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;

namespace Vms.Domain.Entity
{
    public partial class Driver
    {
        public string CompanyCode { get; set; } = null!;
        public string? Salutation { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string LastName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public Geometry HomeLocation { get; set; } = null!;
        public virtual Company CompanyCodeNavigation { get; set; } = null!;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; } = null!;

        public Driver(string companyCode, string? salutation, string? firstName, string? middleNames, string lastName, string emailAddress, string mobileNumber, Geometry homeLocation)
        {
            CompanyCode = companyCode;
            Salutation = salutation;
            FirstName = firstName;
            MiddleNames = middleNames;
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            EmailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
            MobileNumber = mobileNumber ?? throw new ArgumentNullException(nameof(mobileNumber));
            HomeLocation = homeLocation ?? throw new ArgumentNullException(nameof(homeLocation));
        }
        public string FullName() 
        {
            string?[] names = { Salutation, FirstName, MiddleNames, LastName };
            return string.Join(" ", names.Where(x=>!string.IsNullOrEmpty(x)));
        }
    }

    public partial class DriverVehicle
    {
        public string CompanyCode { get; set; } = null!;

        public Guid VehicleId { get; set; }
        public string EmailAddress { get; set; } = null!;

        public virtual Driver EmailAddressNavigation { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
        public DriverVehicle(string companyCode, Guid vehicleId, string emailAddress)
        {
            CompanyCode = companyCode;
            VehicleId = vehicleId;
            EmailAddress = emailAddress;
        }
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class DriverEntityTypeConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.EmailAddress })
                    .HasName("PK_Driver");

            entity.ToTable("Driver");

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.MiddleNames)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.Property(e => e.MobileNumber)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(e => e.Salutation)
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Drivers)
                //.HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Driver_Company");
        }
    }
    public class DriverVehicleEntityTypeConfiguration : IEntityTypeConfiguration<DriverVehicle>
    {
        public void Configure(EntityTypeBuilder<DriverVehicle> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.EmailAddress, e.VehicleId });

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.HasOne(d => d.EmailAddressNavigation)
                .WithMany(p => p.DriverVehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.EmailAddress })
                
                .HasConstraintName("FK_DriverVehicles_Driver");

            entity.HasOne(d => d.Vehicle)
                .WithMany(p => p.DriverVehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.VehicleId })
                .HasPrincipalKey(d => new { d.CompanyCode, d.Id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverVehicles_Vehicle");
        }
    }
}