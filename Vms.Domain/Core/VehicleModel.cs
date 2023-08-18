namespace Vms.Domain.Core
{
    [Table("VehicleModels")]
    public class VehicleModel
    {
        public const int Model_MaxLength = 50;

        public string Make { get; set; } = null!;
        public VehicleMake VehicleMake { get; private set; } = null!;

        [StringLength(Model_MaxLength)]
        public string Model { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();

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

            builder.HasOne(d => d.VehicleMake)
                .WithMany(p => p.VehicleModels)
                .HasForeignKey(d => d.Make)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}