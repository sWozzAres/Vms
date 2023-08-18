namespace Vms.Domain.Core
{
    [Table("Customers")]
    public class Customer
    {
        public const int Code_MaxLength = 10;

        public string CompanyCode { get; set; } = null!;
        public Company Company { get; private set; } = null!;

        [StringLength(Code_MaxLength)]
        public string Code { get; set; } = null!;

        [StringLength(32)]
        public string Name { get; set; } = null!;

        public ICollection<CustomerNetwork> CustomerNetworks { get; } = new List<CustomerNetwork>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();

        private Customer() { }
        public Customer(string companyCode, string code, string name) 
            => (CompanyCode, Code, Name) = (companyCode, code, name);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(e => new { e.CompanyCode, e.Code });

            builder.Property(e => e.Code).IsFixedLength();

            builder.HasOne(d => d.Company)
                .WithMany(d => d.Customers)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customers_Companies");
        }
    }
}
