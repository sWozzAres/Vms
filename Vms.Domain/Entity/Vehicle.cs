using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Runtime.Serialization;

namespace Vms.Domain.Entity
{
    public partial class Vehicle
    {
        public string CompanyCode { get; private set; } = null!;
        public Guid Id { get; private set; }
        public string Make { get; private set; } = null!;
        public string Model { get; private set; } = null!;
        public string? ChassisNumber { get; private set; }
        public DateOnly DateFirstRegistered { get; private set; }
        public Address Address { get; private set; } = null!;
        public string? CustomerCode { get; private set; }
        public string? FleetCode { get; private set; }

        public virtual Customer? C { get; set; }
        public virtual Company CompanyCodeNavigation { get; set; } = null!;
        public virtual Fleet? Fleet { get; set; }
        public virtual VehicleMot Mot { get; set; } = null!;
        public virtual VehicleVrm VehicleVrm { get; set; } = null!;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; } = null!;
        public virtual VehicleModel M { get; set; } = null!;
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = null!;
        private Vehicle() { }
        private Vehicle(string companyCode, string vrm, string make, string model, DateOnly dateFirstRegistered, DateOnly motDue, Address homeLocation)
        {
            CompanyCode = companyCode;
            Id = Guid.NewGuid();
            Make = make;
            Model = model;
            DateFirstRegistered = dateFirstRegistered;
            Mot = new VehicleMot(motDue);
            VehicleVrm = new VehicleVrm(vrm);
            Address = new(homeLocation.Street, homeLocation.Locality, homeLocation.Town, homeLocation.Postcode, homeLocation.Location.Copy());
        }
        public static Vehicle Create(string companyCode, string vrm, string make, string model, DateOnly dateFirstRegistered, DateOnly motDue, Address homeLocation,
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
        public void AssignToCustomer(string customerCode) => CustomerCode = customerCode;
        public void AssignToFleet(string fleetCode) => FleetCode = fleetCode;
    }

    public partial class VehicleMot
    {
        public Guid VehicleId { get; private set; }
        public DateOnly Due { get; private set; }
        public virtual Vehicle Vehicle { get; private set; } = null!;
        private VehicleMot() { }
        public VehicleMot(DateOnly due) => Due = due;
    }

    public partial class VehicleVrm
    {
        public Guid VehicleId { get; private set; }
        public string Vrm { get; internal set; } = null!;
        public virtual Vehicle Vehicle { get; private set; } = null!;
        private VehicleVrm() { }
        public VehicleVrm(string vrm) => Vrm = vrm;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class VehicleEntityTypeConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicle");
            builder.HasKey(e => e.Id);
            //builder.Property(e => e.Id).UseHiLo("VehicleIds");

            builder.Ignore(e => e.Vrm);

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.FleetCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.Property(e => e.ChassisNumber)
                .HasMaxLength(18)
                .IsUnicode(false);

            builder.Property(e => e.DateFirstRegistered).HasColumnType("date");

            builder.Property(e => e.Make)
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.Property(e => e.Model)
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

            builder.HasOne(d => d.M)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.Make, d.Model })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleModel");

            builder.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Vehicles)
                //.HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Company");

            builder.HasOne(d => d.C).WithMany(p => p.Vehicles)
                //.HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .HasConstraintName("FK_Vehicle_Customer");

            builder.HasOne(d => d.Fleet).WithMany(p => p.Vehicles)
                //.HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .HasConstraintName("FK_Vehicle_Fleet");

            builder.HasMany(d => d.ServiceBookings).WithOne(p => p.Vehicle);

            builder.OwnsOne(d => d.Mot, x =>
            {
                x.ToTable("VehicleMot");
                x.WithOwner(x => x.Vehicle)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Vehicle_VehicleMot");
            });
            builder.OwnsOne(d => d.VehicleVrm, x =>
            {
                x.ToTable("VehicleVrm", tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("VehicleVrmHistory");
                    ttb
                        .HasPeriodStart("ValidFrom")
                        .HasColumnName("ValidFrom");
                    ttb
                        .HasPeriodEnd("ValidTo")
                        .HasColumnName("ValidTo");
                }));

                x.WithOwner(x => x.Vehicle)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Vehicle_VehicleVrm");
            });
        }
    }
}