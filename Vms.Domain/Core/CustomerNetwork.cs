namespace Vms.Domain.Core
{
    [Table("CustomerNetworks")]
    public partial class CustomerNetwork
    {
        public string CompanyCode { get; set; } = null!;

        [StringLength(Customer.Code_MaxLength)]
        public string CustomerCode { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

        [StringLength(Network.Code_MaxLength)]
        public string NetworkCode { get; set; } = null!;
        public Network Network { get; set; } = null!;

        private CustomerNetwork() { }
        public CustomerNetwork(string companyCode, string customerCode, string networkCode)
        {
            CustomerCode = customerCode;
            NetworkCode = networkCode;
            CompanyCode = companyCode;
        }
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class CustomerNetworkEntityTypeConfiguration : IEntityTypeConfiguration<CustomerNetwork>
    {
        public void Configure(EntityTypeBuilder<CustomerNetwork> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.NetworkCode, e.CustomerCode });

            builder.HasOne(d => d.Customer)
                .WithMany(p => p.CustomerNetworks)
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerNetworks_Customers");

            builder.HasOne(d => d.Network)
                .WithMany(p => p.CustomerNetworks)
                .HasForeignKey(d => new { d.CompanyCode, d.NetworkCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerNetworks_Networks");
        }
    }
}