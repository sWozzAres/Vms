using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Customer : IMultiTenantEntity
    {
        public string CompanyCode { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public virtual Company CompanyCodeNavigation { get; set; } = null!;
        public virtual ICollection<CustomerNetwork> CustomerNetworks { get; set; } = new List<CustomerNetwork>();
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        private Customer() { }
        public Customer(string companyCode, string code, string name) => (CompanyCode, Code, Name) = (companyCode, code, name);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer");
            
            builder.HasKey(e => new { e.CompanyCode, e.Code });

            builder.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);

            builder.HasOne(d => d.CompanyCodeNavigation).WithMany(d=>d.Customers)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Company");
        }
    }
}
