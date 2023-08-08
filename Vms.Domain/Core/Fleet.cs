namespace Vms.Domain.Core
{
    public partial class Fleet
    {
        public string CompanyCode { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public virtual Company CompanyCodeNavigation { get; set; } = null!;

        public virtual ICollection<FleetNetwork> FleetNetworks { get; set; } = new List<FleetNetwork>();

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        private Fleet() { }
        public Fleet(string companyCode, string code, string name) => (CompanyCode, Code, Name) = (companyCode, code, name);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class FleetEntityTypeConfiguration : IEntityTypeConfiguration<Fleet>
    {
        public void Configure(EntityTypeBuilder<Fleet> entity)
        {
            entity.ToTable("Fleets");

            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(Company.Code_MaxLength)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.CompanyCodeNavigation).WithMany(p => p.Fleets)
                //.HasPrincipalKey(p => p.Code)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fleets_Companies");
        }
    }
}