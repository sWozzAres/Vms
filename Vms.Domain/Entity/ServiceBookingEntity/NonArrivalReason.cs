﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public class NonArrivalReason
    {
        public string CompanyCode { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        internal Company Company { get; set; } = null!;
        public NonArrivalReason(string companyCode, string code, string name)
        {
            CompanyCode = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public class NonArrivalReasonEntityTypeConfiguration : IEntityTypeConfiguration<NonArrivalReason>
    {
        public void Configure(EntityTypeBuilder<NonArrivalReason> entity)
        {
            entity.ToTable("NonArrivalReasons");

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

            entity.HasOne(d => d.Company).WithMany(p => p.NonArrivalReasons)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NonArrivalReason_Company");
        }
    }
}