using Vms.Domain.ServiceBookingProcess;

namespace Vms.Domain.Core
{
    [Table("Vehicles")]
    public class Vehicle
    {
        public string CompanyCode { get; private set; } = null!;
        public Company Company { get; private set; } = null!;

        [Key]
        public Guid Id { get; private set; }

        public string Make { get; private set; } = null!;
        public string Model { get; private set; } = null!;
        public VehicleModel VehicleModel { get; private set; } = null!;

        [StringLength(18)]
        public string? ChassisNumber { get; internal set; }

        public DateOnly DateFirstRegistered { get; set; }

        public Address Address { get; private set; } = null!;

        public string? CustomerCode { get; private set; }
        public Customer? Customer { get; private set; }

        public string? FleetCode { get; private set; }
        public Fleet? Fleet { get; private set; }

        public VehicleVrm VehicleVrm { get; private set; } = null!;

        public ICollection<DriverVehicle> DriverVehicles { get; } = new List<DriverVehicle>();
        public ICollection<ServiceBooking> ServiceBookings { get; } = new List<ServiceBooking>();
        public ICollection<MotEvent> MotEvents { get; } = new List<MotEvent>();
        public ICollection<ServiceEvent> ServiceEvents { get; } = new List<ServiceEvent>();

        private Vehicle() { }
        private Vehicle(string companyCode, string vrm, string make, string model,
            DateOnly dateFirstRegistered, DateOnly? motDue, Address homeLocation)
        {
            CompanyCode = companyCode;
            Id = Guid.NewGuid();
            Make = make;
            Model = model;
            DateFirstRegistered = dateFirstRegistered;

            MotEvents.Add(new(CompanyCode, Id,
                motDue ?? dateFirstRegistered.AddYears(3),
                true));

            VehicleVrm = new VehicleVrm(vrm);

            Address = new(homeLocation);
        }
        public static Vehicle Create(string companyCode, string vrm, string make, string model,
            DateOnly dateFirstRegistered, DateOnly? motDue, Address homeLocation,
            string? customerCode = null, string? fleetCode = null)
        {
            var vehicle = new Vehicle(companyCode, vrm, make, model, dateFirstRegistered, motDue,
                homeLocation);

            if (customerCode is not null)
                vehicle.AssignToCustomer(customerCode);

            if (fleetCode is not null)
                vehicle.AssignToFleet(fleetCode);

            return vehicle;
        }

        public string Vrm
        {
            get => VehicleVrm.Vrm;
            set => VehicleVrm.Vrm = value;
        }
        internal void AssignToCustomer(string customerCode) => CustomerCode = customerCode;
        internal void RemoveCustomer() => CustomerCode = null;
        public void AssignToFleet(string fleetCode) => FleetCode = fleetCode;
        internal void RemoveFleet() => FleetCode = null;
        public void SetMakeModel(string make, string model)
        {
            Make = make;
            Model = model;
        }
        public void SetAddress(string street, string locality, string town, string postcode, double latitude, double longitude)
            => Address = new Address(street, locality, town, postcode, new Point(longitude, latitude) { SRID = 4326 });

    }

    public class VehicleVrm
    {
        public Guid VehicleId { get; private set; }
        public Vehicle Vehicle { get; private set; } = null!;

        [StringLength(12)]
        public string Vrm { get; internal set; } = null!;

        private VehicleVrm() { }
        public VehicleVrm(string vrm) => Vrm = vrm;
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class VehicleEntityTypeConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasAlternateKey(e => new { e.CompanyCode, e.Id });

            builder.Ignore(e => e.Vrm);

            builder.Property(e => e.DateFirstRegistered)
                .HasColumnType("date");

            builder.OwnsOne(e => e.Address);

            builder.HasOne(d => d.VehicleModel)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.Make, d.Model })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_VehicleModels");

            builder.HasOne(d => d.Company)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicles_Companies");

            builder.HasOne(d => d.Customer)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .HasConstraintName("FK_Vehicles_Customers");

            builder.HasOne(d => d.Fleet)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .HasConstraintName("FK_Vehicles_Fleets");

            builder.HasMany(d => d.ServiceBookings)
                .WithOne(p => p.Vehicle);

            builder.OwnsOne(d => d.VehicleVrm, x =>
            {
                x.ToTable("VehicleVrms", tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("VehicleVrmsHistory");
                    ttb
                        .HasPeriodStart("ValidFrom")
                        .HasColumnName("ValidFrom");
                    ttb
                        .HasPeriodEnd("ValidTo")
                        .HasColumnName("ValidTo");
                }));

                x.WithOwner(x => x.Vehicle)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Vehicles_VehicleVrms");
            });
        }
    }
}