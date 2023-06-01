using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vms.Domain.Entity
{
    public partial class Vehicle : IMultiTenantEntity
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
        public virtual VehicleMot Mot { get; set; } = null!;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; } = null!;
        public virtual VehicleModel M { get; set; } = null!;
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = null!;
        private Vehicle() { }
        public Vehicle(string companyCode, string vrm, string make, string model, DateOnly dateFirstRegistered, DateOnly motDue)
        {
            CompanyCode = companyCode;
            Make = make;
            Model = model;
            DateFirstRegistered = dateFirstRegistered;
            Mot = new VehicleMot(motDue);
            VehicleVrm = new VehicleVrm(vrm);
        }
        
        public string Vrm
        {
            get => VehicleVrm.Vrm;
            set => VehicleVrm.Vrm = value;
        }
    }

    public partial class VehicleMot
    {
        public int VehicleId { get; set; }
        public DateOnly Due { get; set; }
        public virtual Vehicle Vehicle { get; set; } = null!;
        private VehicleMot() { }
        public VehicleMot(DateOnly due) => Due = due;
    }

    public partial class VehicleVrm
    {
        public int VehicleId { get; set; }
        public string Vrm { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
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
            builder.HasAlternateKey(e => e.Id);
            builder.Property(e => e.Id).UseHiLo("VehicleIds");

            builder.Ignore(e => e.Vrm);

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