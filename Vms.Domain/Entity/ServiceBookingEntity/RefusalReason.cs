﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public class RefusalReason(string companyCode, string code, string name)
    {
        public string CompanyCode { get; set; } = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
        public string Code { get; set; } = code ?? throw new ArgumentNullException(nameof(code));
        public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));
        internal Company Company { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public class RefusalReasonEntityTypeConfiguration : IEntityTypeConfiguration<RefusalReason>
    {
        public void Configure(EntityTypeBuilder<RefusalReason> entity)
        {
            entity.ToTable("RefusalReasons");

            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.RefusalReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefusalReasons_Companies");
        }
    }
}