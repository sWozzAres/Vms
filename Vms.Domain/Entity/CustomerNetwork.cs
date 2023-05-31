using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class CustomerNetwork
    {
        public string CustomerCode { get; set; } = null!;

        public string NetworkCode { get; set; } = null!;

        public string CompanyCode { get; set; } = null!;

        public virtual Customer C { get; set; } = null!;

        public virtual Network Network { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class CustomerNetworkEntityTypeConfiguration : IEntityTypeConfiguration<CustomerNetwork>
    {
        public void Configure(EntityTypeBuilder<CustomerNetwork> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.NetworkCode, e.CustomerCode });

            builder.ToTable("CustomerNetwork");

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.NetworkCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.HasOne(d => d.C).WithMany(p => p.CustomerNetworks)
                .HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerNetwork_Customer");

            builder.HasOne(d => d.Network).WithMany(p => p.CustomerNetworks)
                .HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.NetworkCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerNetwork_Network");
        }
    }
}    