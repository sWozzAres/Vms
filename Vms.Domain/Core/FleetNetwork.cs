namespace Vms.Domain.Core
{
    [Table("FleetNetworks")]
    public class FleetNetwork
    {
        public string CompanyCode { get; set; } = null!;

        public string FleetCode { get; set; } = null!;
        public Fleet Fleet { get; private set; } = null!;

        public string NetworkCode { get; set; } = null!;
        public Network Network { get; private set; } = null!;

        private FleetNetwork() { }
        public FleetNetwork(string companyCode, string fleetCode, string networkCode)
        {
            FleetCode = fleetCode;
            NetworkCode = networkCode;
            CompanyCode = companyCode;
        }
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class FleetNetworkEntityTypeConfiguration : IEntityTypeConfiguration<FleetNetwork>
    {
        public void Configure(EntityTypeBuilder<FleetNetwork> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.FleetCode, e.NetworkCode });

            builder.HasOne(d => d.Fleet)
                .WithMany(p => p.FleetNetworks)
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FleetNetworks_Fleets");

            builder.HasOne(d => d.Network)
                .WithMany(p => p.FleetNetworks)
                .HasForeignKey(d => new { d.CompanyCode, d.NetworkCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FleetNetworks_Networks");
        }
    }
}