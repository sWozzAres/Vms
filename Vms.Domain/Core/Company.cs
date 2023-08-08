using Vms.Domain.ServiceBookingProcess;

namespace Vms.Domain.Core
{
    public partial class Company
    {
        public const int Code_MaxLength = 10;
        public string Code { get; private set; } = null!;

        public string Name { get; set; } = null!;

        internal List<Customer> Customers = new();

        internal List<Fleet> Fleets { get; private set; } = new();

        internal List<Network> Networks { get; private set; } = new();

        internal List<Vehicle> Vehicles { get; private set; } = new();
        internal List<Driver> Drivers { get; private set; } = new();
        internal List<NonArrivalReason> NonArrivalReasons { get; private set; } = new();
        internal List<NotCompleteReason> NotCompleteReasons { get; private set; } = new();
        internal List<RefusalReason> RefusalReasons { get; private set; } = new();
        internal List<ConfirmBookedRefusalReason> ConfirmBookedRefusalReasons { get; private set; } = new();
        internal List<RescheduleReason> RescheduleReasons { get; private set; } = new();
        private Company() { }
        internal Company(string code, string name) => (Code, Name) = (code, name);
    }
}

namespace Vms.Domain.Core.Configuration
{
    public class CompanyEntityTypeConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code)
                .HasMaxLength(Company.Code_MaxLength)
                .IsFixedLength();

            builder.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        }
    }
}

