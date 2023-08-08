using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    public class MotEvent
    {
        public Guid Id { get; set; }
        public Guid? ServiceBookingId { get; set; }
        public ServiceBooking? ServiceBooking { get; set; } = null!;
        public string CompanyCode { get; set; } = null!;
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public DateOnly Due { get; set; }
        public bool IsCurrent { get; set; }
        private MotEvent() { }
        public MotEvent(string companyCode, Guid vehicleId, DateOnly due, bool isCurrent)
        {
            Id = Guid.NewGuid();
            CompanyCode = companyCode ?? throw new ArgumentNullException(nameof(companyCode));
            VehicleId = vehicleId;
            Due = due;
            IsCurrent = isCurrent;
        }
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class ServiceBookingMotEventEntityTypeConfiguration : IEntityTypeConfiguration<MotEvent>
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

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.VehicleId, e.IsCurrent })
                .IsUnique()
                .HasFilter("IsCurrent = 1");

            entity.HasOne(e => e.ServiceBooking)
                .WithOne(s => s.MotEvent)
                .HasForeignKey<MotEvent>(e => new { e.CompanyCode, e.VehicleId, e.ServiceBookingId })
                .HasPrincipalKey<ServiceBooking>(v => new { v.CompanyCode, v.VehicleId, v.Id });


            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.MotEvents)
                .HasForeignKey(e => new { e.CompanyCode, e.VehicleId })
                .HasPrincipalKey(v => new { v.CompanyCode, v.Id })
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}