using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Vehicle
    {
        public string CompanyCode { get; set; } = null!;
        public int Id { get; set; }
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? ChassisNumber { get; set; }
        public DateOnly DateFirstRegistered { get; set; }
        public virtual VehicleVrm VehicleVrm { get; set; } = null!;
        public string? CustomerCode { get; set; }
        public string? FleetCode { get; set; }
        public virtual Customer? C { get; set; }
        public virtual Company CompanyCodeNavigation { get; set; } = null!;
        public virtual Fleet? Fleet { get; set; }
        public virtual NextMot NextMot { get; set; } = null!;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; } = null!;
        public virtual VehicleModel M { get; set; } = null!;
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = null!;
        private Vehicle() { }
        public Vehicle(string vrm, string make, string model, DateOnly dateFirstRegistered, DateOnly motDue, IEnumerable<Driver> drivers)
        {
            Make = make;
            Model = model;
            DateFirstRegistered = dateFirstRegistered;

            NextMot = new NextMot() { Due =  motDue };

            DriverVehicles = new List<DriverVehicle>();
            foreach(var driver in drivers)
            {
                DriverVehicles.Add(new DriverVehicle() { EmailAddress = driver.EmailAddress });
            }

            VehicleVrm = new VehicleVrm() { Vrm = vrm };
        }
    }

    public partial class NextMot
    {
        public int VehicleId { get; set; }
        public DateOnly Due { get; set; }
        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class VehicleEntityTypeConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicle");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();
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

            builder.HasOne(d => d.M)
                .WithMany(p => p.Vehicles)
                .HasForeignKey(d => new { d.Make, d.Model })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleModel");

            builder.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Vehicles)
                .HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_Company");

            //builder.HasOne(d => d.NextMot).WithOne(p => p.Vehicle)
            //    .HasForeignKey<Vehicle>(d => d.Id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Vehicle_NextMot");

            builder.HasOne(d => d.C).WithMany(p => p.Vehicles)
                .HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .HasConstraintName("FK_Vehicle_Customer");

            builder.HasOne(d => d.Fleet).WithMany(p => p.Vehicles)
                .HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .HasConstraintName("FK_Vehicle_Fleet");

            builder.HasMany(d => d.ServiceBookings).WithOne(p => p.Vehicle);

            builder.OwnsOne(d => d.NextMot, x =>
            {
                x.ToTable("NextMot");
                x.WithOwner(x => x.Vehicle)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Vehicle_NextMot");
            });
        }
    }
    //public class NextMotEntityTypeConfiguration : IEntityTypeConfiguration<NextMot>
    //{
    //    public void Configure(EntityTypeBuilder<NextMot> builder)
    //    {
    //        builder.ToTable("NextMot");
    //    }
    //}
}