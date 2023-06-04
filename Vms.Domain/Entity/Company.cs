using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Company
    {
        public string Code { get; private set; } = null!;

        public string Name { get; private set; } = null!;

        internal List<Customer> Customers = new();

        internal List<Fleet> Fleets { get; private set; } = new();

        internal List<Network> Networks { get; private set; } = new();

        internal List<Vehicle> Vehicles { get; private set; } = new();

        private Company() { }
        internal Company(string code, string name) => (Code, Name) = (code, name);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class CompanyEntityTypeConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company");

            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            
            builder.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        }
    }
}