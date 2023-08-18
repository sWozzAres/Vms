namespace Vms.Domain.Core
{
    [Table("Fleets")]
    public class Fleet
    {
        public const int Code_MaxLength = 10;

        public string CompanyCode { get; set; } = null!;
        public Company Company { get; private set; } = null!;

        [StringLength(Code_MaxLength)]
        public string Code { get; set; } = null!;

        [StringLength(32)]
        public string Name { get; set; } = null!;

        public ICollection<FleetNetwork> FleetNetworks { get; } = new List<FleetNetwork>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();

        private Fleet() { }
        public Fleet(string companyCode, string code, string name)
            => (CompanyCode, Code, Name) = (companyCode, code, name);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class FleetEntityTypeConfiguration : IEntityTypeConfiguration<Fleet>
    {
        public void Configure(EntityTypeBuilder<Fleet> entity)
        {
            entity.HasKey(e => new { e.CompanyCode, e.Code });

            entity.Property(e => e.Code).IsFixedLength();

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Fleets)
                .HasForeignKey(d => d.CompanyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fleets_Companies");
        }
    }
}