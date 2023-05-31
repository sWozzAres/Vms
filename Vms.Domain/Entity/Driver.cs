using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using Vms.Domain.Entity;

namespace Vms.Domain.Entity
{
    public partial class Driver
    {
        public string? Salutation { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleNames { get; set; }
        public string LastName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public Geometry HomeLocation { get; set; } = null!;

        public virtual DriverVehicle DriverVehicle { get; set; } = null!;
    }

    public partial class DriverVehicle
    {
        public int VehicleId { get; set; }
        public string EmailAddress { get; set; } = null!;

        public virtual Driver EmailAddressNavigation { get; set; } = null!;
        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class DriverEntityTypeConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> entity)
        {
            entity.HasKey(e => e.EmailAddress)
                    .HasName("PK_Driver_1");

            entity.ToTable("Driver");

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
        }
    }
    public class DriverVehicleEntityTypeConfiguration : IEntityTypeConfiguration<DriverVehicle>
    {
        public void Configure(EntityTypeBuilder<DriverVehicle> entity)
        {
            entity.HasKey(e => e.EmailAddress);

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(128)
                .IsUnicode(false);

            entity.HasOne(d => d.EmailAddressNavigation)
                .WithOne(p => p.DriverVehicle)
                .HasForeignKey<DriverVehicle>(d => d.EmailAddress)
                .HasConstraintName("FK_DriverVehicles_Driver");

            entity.HasOne(d => d.Vehicle)
                .WithMany(p => p.DriverVehicles)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverVehicles_Vehicle");
        }
    }
}