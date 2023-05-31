using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class FleetNetwork
    {
        public string FleetCode { get; set; } = null!;

        public string NetworkCode { get; set; } = null!;

        public string CompanyCode { get; set; } = null!;

        public virtual Fleet Fleet { get; set; } = null!;

        public virtual Network Network { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class FleetNetworkEntityTypeConfiguration : IEntityTypeConfiguration<FleetNetwork>
    {
        public void Configure(EntityTypeBuilder<FleetNetwork> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.FleetCode, e.NetworkCode });

            builder.ToTable("FleetNetwork");

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.FleetCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.NetworkCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.HasOne(d => d.Fleet).WithMany(p => p.FleetNetworks)
                .HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FleetNetwork_Fleet");

            builder.HasOne(d => d.Network).WithMany(p => p.FleetNetworks)
                .HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.NetworkCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FleetNetwork_Network");
        }
    }
}