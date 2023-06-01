using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Vms.Domain.Entity
{
    public partial class Company
    {
        public string Code { get; set; } = null!;

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

        public virtual ICollection<Fleet> Fleets { get; set; } = new List<Fleet>();

        public virtual ICollection<Network> Networks { get; set; } = new List<Network>();

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public string CompanyCode { get => Code; set => Code = value; }

        private Company() { }
        public Company(string code, string name) => (Code, Name) = (code, name);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class CompanyEntityTypeConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Company");

            builder.Property(e => e.Id).UseHiLo("CompanyIds");

            builder.HasIndex(e => e.Code, "IX_Company").IsUnique();

            builder.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            builder.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        }
    }
}