using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public partial class VehicleMake
    {
        internal const int Make_Maxlength = 30;
        public string Make { get; set; } = null!;
        public virtual ICollection<VehicleModel> VehicleModels { get; set; } = null!;
        
        private VehicleMake() { }
        public VehicleMake(string make)
        {
            Make = make;
            VehicleModels = new List<VehicleModel>();
        }
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
                .HasMaxLength(VehicleMake.Make_Maxlength)
                .IsUnicode(false);
        }
    }
}