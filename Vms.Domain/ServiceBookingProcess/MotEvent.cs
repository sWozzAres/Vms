using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    public class MotEvent
    {
        [Key]
        public Guid Id { get; private set; }

        public string CompanyCode { get; private set; } = null!;

        public Guid? ServiceBookingId { get; private set; }
        public ServiceBooking? ServiceBooking { get; private set; } = null!;

        public Guid VehicleId { get; private set; }
        public Vehicle Vehicle { get; private set; } = null!;

        public DateOnly Due { get; set; }
        public bool IsCurrent { get; private set; }

        private MotEvent() { }
        public MotEvent(string companyCode, Guid vehicleId, DateOnly due, bool isCurrent = true)
        {
            Id = Guid.NewGuid();
            CompanyCode = companyCode;
            VehicleId = vehicleId;
            Due = due;
            IsCurrent = isCurrent;
        }
        internal void AddToServiceBooking(ServiceBooking serviceBooking)
        {
            ServiceBooking = serviceBooking;
            ServiceBookingId = serviceBooking.Id;
        }
        internal void RemoveFromServiceBooking()
        {
            ServiceBooking = null;
            ServiceBookingId = null;
        }
        internal void Complete() => IsCurrent = false;
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class MotEventEntityTypeConfiguration : IEntityTypeConfiguration<MotEvent>
    {
        public void Configure(EntityTypeBuilder<MotEvent> entity)
        {
            entity.ToTable("MotEvents", tb => tb.IsTemporal(ttb =>
            {
                ttb.UseHistoryTable("MotEventsHistory");
                ttb.HasPeriodStart("ValidFrom")
                   .HasColumnName("ValidFrom");
                ttb.HasPeriodEnd("ValidTo")
                   .HasColumnName("ValidTo");
            }));

            entity.HasIndex(e => new { e.VehicleId, e.IsCurrent })
                .IsUnique()
                .HasFilter("IsCurrent = 1");

            entity.HasOne(e => e.ServiceBooking)
                .WithOne(s => s.MotEvent)
                .HasForeignKey<MotEvent>(e => new { e.CompanyCode, e.VehicleId, e.ServiceBookingId })
                .HasPrincipalKey<ServiceBooking>(v => new { v.CompanyCode, v.VehicleId, v.Id })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.MotEvents)
                .HasForeignKey(e => new { e.CompanyCode, e.VehicleId })
                .HasPrincipalKey(v => new { v.CompanyCode, v.Id })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}