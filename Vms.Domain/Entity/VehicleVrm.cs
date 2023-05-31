using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class VehicleVrm
    {
        public int VehicleId { get; set; }
        public string Vrm { get; set; } = null!;

        public virtual Vehicle Vehicle { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class VehicleVrmEntityTypeConfiguration : IEntityTypeConfiguration<VehicleVrm>
    {
        public void Configure(EntityTypeBuilder<VehicleVrm> builder)
        {
            builder.HasKey(e => e.VehicleId)
                    .HasName("PK__VehicleVrm");

            builder.ToTable("VehicleVrm");

            builder.ToTable(tb => tb.IsTemporal(ttb =>
            {
                ttb.UseHistoryTable("VehicleVrmHistory", "dbo");
                ttb
                    .HasPeriodStart("ValidFrom")
                    .HasColumnName("ValidFrom");
                ttb
                    .HasPeriodEnd("ValidTo")
                    .HasColumnName("ValidTo");
            }
            ));

            builder.Property(e => e.VehicleId).ValueGeneratedNever();

            builder.Property(e => e.Vrm)
                .HasMaxLength(8)
                .IsUnicode(false);

            builder.HasOne(d => d.Vehicle)
                .WithOne(p => p.VehicleVrm)
                .HasForeignKey<VehicleVrm>(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleVrm_Vehicle");
        }
    }
}