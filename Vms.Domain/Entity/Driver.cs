using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Vms.Domain.Entity
{
    public partial class Driver(string companyCode, string? salutation, string? firstName, string? middleNames, string lastName, string emailAddress, string mobileNumber, Geometry homeLocation)
    {
        public const int FirstName_MaxLength = 20;
        public const int LastName_MaxLength = 20;
        public const int Email_MaxLength = 128;
        public const int MobileNumber_MaxLength = 12;
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CompanyCode { get; set; } = companyCode;
        public string? Salutation { get; set; } = salutation;
        public string? FirstName { get; set; } = firstName;
        public string? MiddleNames { get; set; } = middleNames;
        public string LastName { get; set; } = lastName ?? throw new ArgumentNullException(nameof(lastName));
        public string EmailAddress { get; set; } = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
        public string MobileNumber { get; set; } = mobileNumber ?? throw new ArgumentNullException(nameof(mobileNumber));
        public Geometry HomeLocation { get; set; } = homeLocation ?? throw new ArgumentNullException(nameof(homeLocation));
        public virtual Company CompanyCodeNavigation { get; set; } = null!;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; } = null!;

        public string FullName
            => string.Join(" ", new string?[] { Salutation, FirstName, MiddleNames, LastName }
                .Where(x => !string.IsNullOrEmpty(x)));
    }

    public partial class DriverVehicle(string companyCode, Guid driverId, Guid vehicleId)
    {
        public string CompanyCode { get; set; } = companyCode;
        public Guid VehicleId { get; set; } = vehicleId;
        public Guid DriverId { get; set; } = driverId;

        public virtual Driver Driver { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class DriverEntityTypeConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> entity)
        {
            entity.HasKey(e => e.Id);
            entity.HasAlternateKey(e => new { e.CompanyCode, e.Id });
            entity.HasAlternateKey(e => new { e.CompanyCode, e.EmailAddress });

            entity.ToTable("Drivers");

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
                .HasConstraintName("FK_Drivers_Companies");
        }
    }
    public class DriverVehicleEntityTypeConfiguration : IEntityTypeConfiguration<DriverVehicle>
    {
        public void Configure(EntityTypeBuilder<DriverVehicle> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.DriverId, e.VehicleId });

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();

            //entity.Property(e => e.EmailAddress)
            //    .HasMaxLength(128)
            //    .IsUnicode(false);

            entity.HasOne(d => d.Driver)
                .WithMany(p => p.DriverVehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.DriverId })
                .HasPrincipalKey(d => new { d.CompanyCode, d.Id })
                .HasConstraintName("FK_DriverVehicles_Drivers");

            entity.HasOne(d => d.Vehicle)
                .WithMany(p => p.DriverVehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.VehicleId })
                .HasPrincipalKey(d => new { d.CompanyCode, d.Id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverVehicles_Vehicles");
        }
    }
}