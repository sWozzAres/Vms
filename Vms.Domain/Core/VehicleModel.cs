namespace Vms.Domain.Core
{
    public partial class VehicleModel
    {
        internal const int Model_MaxLength = 50;
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;

        public virtual VehicleMake MakeNavigation { get; set; } = null!;
        public virtual ICollection<Vehicle> Vehicles { get; set; } = null!;
        private VehicleModel() { }
        public VehicleModel(string make, string model)
            => (Make, Model) = (make, model);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class VehicleModelEntityTypeConfiguration : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            builder.HasKey(e => new { e.Make, e.Model })
                .HasName("PK_Model");

            builder.ToTable("VehicleModels");

            builder.Property(e => e.Make)
                .HasMaxLength(VehicleMake.Make_Maxlength)
                .IsUnicode(false);

            builder.Property(e => e.Model)
                .HasMaxLength(VehicleModel.Model_MaxLength)
                .IsUnicode(false);

            builder.HasOne(d => d.MakeNavigation)
                .WithMany(p => p.VehicleModels)
                .HasForeignKey(d => d.Make)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}