using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public partial class CustomerNetwork
    {
        public string CustomerCode { get; set; } = null!;

        public string NetworkCode { get; set; } = null!;

        public string CompanyCode { get; set; } = null!;

        public virtual Customer C { get; set; } = null!;

        public virtual Network Network { get; set; } = null!;
        private CustomerNetwork() { }
        public CustomerNetwork(string companyCode, string customerCode, string networkCode)
        {
            CustomerCode = customerCode ?? throw new ArgumentNullException(nameof(customerCode));
            NetworkCode = networkCode ?? throw new ArgumentNullException(nameof(networkCode));
            CompanyCode = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
        }
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class CustomerNetworkEntityTypeConfiguration : IEntityTypeConfiguration<CustomerNetwork>
    {
        public void Configure(EntityTypeBuilder<CustomerNetwork> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.NetworkCode, e.CustomerCode });

            builder.ToTable("CustomerNetworks");

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.NetworkCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.CustomerCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.HasOne(d => d.C).WithMany(p => p.CustomerNetworks)
                //.HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.CustomerCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerNetworks_Customers");

            builder.HasOne(d => d.Network).WithMany(p => p.CustomerNetworks)
                //.HasPrincipalKey(p => new { p.CompanyCode, p.Code })
                .HasForeignKey(d => new { d.CompanyCode, d.NetworkCode })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerNetworks_Networks");
        }
    }
}