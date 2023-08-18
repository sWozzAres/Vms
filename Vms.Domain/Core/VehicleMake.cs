namespace Vms.Domain.Core
{
    [Table("VehicleMakes")]
    public class VehicleMake
    {
        public const int Make_Maxlength = 30;

        [Key]
        [StringLength(Make_Maxlength)]
        public string Make { get; set; } = null!;

        public ICollection<VehicleModel> VehicleModels { get; } = new List<VehicleModel>();

        private VehicleMake() { }
        public VehicleMake(string make)
        {
            Make = make;
            VehicleModels = new List<VehicleModel>();
        }
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class VehicleMakeEntityTypeConfiguration : IEntityTypeConfiguration<VehicleMake>
    {
        public void Configure(EntityTypeBuilder<VehicleMake> builder)
        {
        }
    }
}