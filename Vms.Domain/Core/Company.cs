using Vms.Domain.ServiceBookingProcess;

namespace Vms.Domain.Core
{
    [Table("Companies")]
    public class Company
    {
        public const int Code_MaxLength = 10;

        [Key]
        [StringLength(Code_MaxLength)]
        public string Code { get; private set; } = null!;

        [StringLength(32)]
        public string Name { get; set; } = null!;

        public ICollection<Customer> Customers { get; } = new List<Customer>();
        public ICollection<Fleet> Fleets { get; } = new List<Fleet>();
        public ICollection<Network> Networks { get; } = new List<Network>();
        public ICollection<Vehicle> Vehicles { get; } = new List<Vehicle>();
        public ICollection<Driver> Drivers { get; } = new List<Driver>();
        public ICollection<NonArrivalReason> NonArrivalReasons { get; } = new List<NonArrivalReason>();
        public ICollection<NotCompleteReason> NotCompleteReasons { get; } = new List<NotCompleteReason>();
        public ICollection<RefusalReason> RefusalReasons { get; } = new List<RefusalReason>();
        public ICollection<ConfirmBookedRefusalReason> ConfirmBookedRefusalReasons { get; } = new List<ConfirmBookedRefusalReason>();
        public ICollection<RescheduleReason> RescheduleReasons { get; } = new List<RescheduleReason>();

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
            builder.Property(e => e.Code).IsFixedLength();
        }
    }
}

