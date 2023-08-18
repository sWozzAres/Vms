namespace Vms.Domain.Core
{
    [Table("Drivers")]
    public class Driver(string companyCode, string? salutation, string? firstName, string? middleNames, string lastName, string emailAddress, string mobileNumber, Geometry homeLocation)
    {
        public const int FirstName_MaxLength = 20;
        public const int LastName_MaxLength = 20;
        public const int Email_MaxLength = 128;
        public const int MobileNumber_MaxLength = 12;

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string CompanyCode { get; set; } = companyCode;
        public Company Company { get; private set; } = null!;

        [StringLength(5)]
        public string? Salutation { get; set; } = salutation;

        [StringLength(20)]
        public string? FirstName { get; set; } = firstName;

        [StringLength(30)]
        public string? MiddleNames { get; set; } = middleNames;

        [StringLength(20)]
        public string LastName { get; set; } = lastName;

        [StringLength(128)]
        public string EmailAddress { get; set; } = emailAddress;

        [StringLength(12)]
        public string MobileNumber { get; set; } = mobileNumber;

        public Geometry HomeLocation { get; set; } = homeLocation;

        public ICollection<DriverVehicle> DriverVehicles { get; } = new List<DriverVehicle>();

        public string FullName
            => string.Join(" ", new string?[] { Salutation, FirstName, MiddleNames, LastName }
                .Where(x => !string.IsNullOrEmpty(x)));
    }

    public class DriverVehicle(string companyCode, Guid driverId, Guid vehicleId)
    {
        public string CompanyCode { get; set; } = companyCode;
        public Guid VehicleId { get; set; } = vehicleId;
        public Guid DriverId { get; set; } = driverId;

        public Driver Driver { get; private set; } = null!;
        public Vehicle Vehicle { get; private set; } = null!;
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class DriverEntityTypeConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> entity)
        {
            entity.HasKey(e => e.Id);
            entity.HasAlternateKey(e => new { e.CompanyCode, e.Id });
            entity.HasAlternateKey(e => new { e.CompanyCode, e.EmailAddress });

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Drivers)
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