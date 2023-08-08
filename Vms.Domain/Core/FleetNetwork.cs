namespace Vms.Domain.Core
{
    public partial class FleetNetwork
    {
        public string FleetCode { get; set; } = null!;

        public string NetworkCode { get; set; } = null!;

        public string CompanyCode { get; set; } = null!;

        public virtual Fleet Fleet { get; set; } = null!;

        public virtual Network Network { get; set; } = null!;
        private FleetNetwork() { }
        public FleetNetwork(string companyCode, string fleetCode, string networkCode)
        {
            FleetCode = fleetCode ?? throw new ArgumentNullException(nameof(fleetCode));
            NetworkCode = networkCode ?? throw new ArgumentNullException(nameof(networkCode));
            CompanyCode = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
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

            builder.ToTable("FleetNetworks");

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(Company.Code_MaxLength)
                .IsFixedLength();
            builder.Property(e => e.FleetCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.NetworkCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.HasOne(d => d.Fleet).WithMany(p => p.FleetNetworks)
                //.HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.FleetCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FleetNetworks_Fleets");

            builder.HasOne(d => d.Network).WithMany(p => p.FleetNetworks)
                //.HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.NetworkCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FleetNetworks_Networks");
        }
    }
}