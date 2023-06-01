using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public partial class Fleet : IMultiTenantEntity
    {
        public string CompanyCode { get; set; } = null!;

        public string Code { get; set; } = null!;

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual Company CompanyCodeNavigation { get; set; } = null!;

        public virtual ICollection<FleetNetwork> FleetNetworks { get; set; } = new List<FleetNetwork>();

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        private Fleet() { }
        public Fleet(string companyCode, string code, string name) => (CompanyCode, Code, Name) = (companyCode, code, name);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class FleetEntityTypeConfiguration : IEntityTypeConfiguration<Fleet>
    {
        public void Configure(EntityTypeBuilder<Fleet> builder)
        {
            builder.ToTable("Fleet");

            builder.HasAlternateKey(e => e.Id);
            builder.Property(e => e.Id).UseHiLo("FleetIds");

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

            builder.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Fleets)
                //.HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fleet_Company");
        }
    }
}