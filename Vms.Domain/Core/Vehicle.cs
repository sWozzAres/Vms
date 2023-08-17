using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Vms.Domain.ServiceBookingProcess;

namespace Vms.Domain.Core
{
    [Table("Vehicles")]
    public partial class Vehicle
    {
        [StringLength(Company.Code_MaxLength)]
        public string CompanyCode { get; private set; } = null!;
        public Company Company { get; set; } = null!;

        [Key]
        public Guid Id { get; private set; }

        [StringLength(VehicleMake.Make_Maxlength)]
        public string Make { get; private set; } = null!;

        [StringLength(VehicleModel.Model_MaxLength)]
        public string Model { get; private set; } = null!;
        public virtual VehicleModel VehicleModel { get; set; } = null!;

        [StringLength(18)]
        public string? ChassisNumber { get; internal set; }

        public DateOnly DateFirstRegistered { get; set; }

        public Address Address { get; set; } = null!;

        [StringLength(Customer.Code_MaxLength)]
        public string? CustomerCode { get; private set; }
        public Customer? C { get; set; }
        
        [StringLength(Fleet.Code_MaxLength)]
        public string? FleetCode { get; private set; }
        public Fleet? Fleet { get; set; }
        
        public VehicleMot Mot { get; set; } = null!;
        public VehicleVrm VehicleVrm { get; set; } = null!;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; } = null!;
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = null!;
        public virtual List<MotEvent> MotEvents { get; set; } = new();
        private Vehicle() { }
        private Vehicle(string companyCode, string vrm, string make, string model,
            DateOnly dateFirstRegistered, DateOnly motDue, Address homeLocation)
        {
            CompanyCode = companyCode;
            Id = Guid.NewGuid();
            Make = make;
            Model = model;
            DateFirstRegistered = dateFirstRegistered;

            Mot = new VehicleMot(Id, motDue); 
            MotEvents.Add(new(CompanyCode, Id, motDue, true));

            VehicleVrm = new VehicleVrm(vrm);
            
            Address = new(homeLocation);
        }
        public static Vehicle Create(string companyCode, string vrm, string make, string model,
            DateOnly dateFirstRegistered, DateOnly motDue, Address homeLocation,
            string? customerCode = null, string? fleetCode = null)
        {
            var vehicle = new Vehicle(companyCode, vrm, make, model, dateFirstRegistered, motDue, homeLocation);

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
        public void UpdateModel(string make, string model)
        {
            Make = make;
            Model = model;
        }
    }

    public partial class VehicleMot
    {
        public Guid VehicleId { get; private set; }
        public DateOnly Due { get; set; }
        internal Vehicle Vehicle { get; set; } = null!;
        //public Guid? ServiceBookingId { get; set; }
        //public ServiceBooking? ServiceBooking { get; set; } = null!;
        private VehicleMot() { }
        internal VehicleMot(Guid vehicleId, DateOnly due) => (VehicleId, Due) = (vehicleId, due);
    }

    public partial class VehicleVrm
    {
        public Guid VehicleId { get; private set; }
        public string Vrm { get; internal set; } = null!;
        public Vehicle Vehicle { get; private set; } = null!;
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

            builder.Property(e => e.CompanyCode).IsFixedLength();
            builder.Property(e => e.CustomerCode).IsFixedLength();
            builder.Property(e => e.FleetCode).IsFixedLength();

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

            builder.HasOne(d => d.C)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .HasConstraintName("FK_Vehicles_Customers");

            builder.HasOne(d => d.Fleet)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .HasConstraintName("FK_Vehicles_Fleets");

            builder.HasMany(d => d.ServiceBookings)
                .WithOne(p => p.Vehicle);

            builder.OwnsOne(d => d.Mot, x =>
            {
                x.ToTable("VehicleMots");
                x.WithOwner(x => x.Vehicle)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Vehicles_VehicleMots");
            });

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

                x.Property(p => p.Vrm).HasMaxLength(12);

                x.WithOwner(x => x.Vehicle)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Vehicles_VehicleVrms");
            });
        }
    }
}