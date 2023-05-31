using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class VehicleModel
    {
        internal const int MODEL_MAXLENGTH = 50;
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;

        public virtual VehicleMake MakeNavigation { get; set; } = null!;
        public virtual ICollection<Vehicle> Vehicles { get; set; } = null!;
        private VehicleModel() { }
        public VehicleModel(string make, string model)
            => (Make, Model) = (make, model);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class VehicleModelEntityTypeConfiguration : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            builder.HasKey(e => new { e.Make, e.Model })
                .HasName("PK_Model");

            builder.ToTable("VehicleModel");

            builder.Property(e => e.Make)
                .HasMaxLength(VehicleMake.MAKE_MAXLENGTH)
                .IsUnicode(false);

            builder.Property(e => e.Model)
                .HasMaxLength(VehicleModel.MODEL_MAXLENGTH)
                .IsUnicode(false);

            builder.HasOne(d => d.MakeNavigation)
                .WithMany(p => p.VehicleModels)
                .HasForeignKey(d => d.Make)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}