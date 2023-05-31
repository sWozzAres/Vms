using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class VehicleMake
    {
        internal const int MAKE_MAXLENGTH = 30;
        public string Make { get; set; } = null!;
        public virtual ICollection<VehicleModel> VehicleModels { get; set; } = null!;
        public virtual ICollection<Supplier> SupplierCodes { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class VehicleMakeEntityTypeConfiguration : IEntityTypeConfiguration<VehicleMake>
    {
        public void Configure(EntityTypeBuilder<VehicleMake> builder)
        {
            builder.HasKey(e => e.Make)
                .HasName("PK_Make");

            builder.ToTable("VehicleMake");

            builder.Property(e => e.Make)
                .HasMaxLength(VehicleMake.MAKE_MAXLENGTH)
                .IsUnicode(false);
        }
    }
}