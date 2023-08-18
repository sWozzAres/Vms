using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    public class ServiceEvent
    {
        [Key]
        public Guid Id { get; set; }

        public string CompanyCode { get; set; } = null!;

        public Guid? ServiceBookingId { get; set; }
        public ServiceBooking? ServiceBooking { get; set; } = null!;

        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        [StringLength(100)]
        public string Description { get; set; } = null!;
        public bool IsCurrent { get; set; }

        private ServiceEvent() { }
        public ServiceEvent(string companyCode, Guid vehicleId, string description, bool isCurrent)
        {
            Id = Guid.NewGuid();
            CompanyCode = companyCode;
            VehicleId = vehicleId;
            Description = description;
            IsCurrent = isCurrent;
        }
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class ServiceEventEntityTypeConfiguration : IEntityTypeConfiguration<ServiceEvent>
    {
        public void Configure(EntityTypeBuilder<ServiceEvent> entity)
        {
            entity.ToTable("ServiceEvents", tb => tb.IsTemporal(ttb =>
            {
                ttb.UseHistoryTable("ServiceEventsHistory");
                ttb.HasPeriodStart("ValidFrom")
                   .HasColumnName("ValidFrom");
                ttb.HasPeriodEnd("ValidTo")
                   .HasColumnName("ValidTo");
            }));

            entity.HasIndex(e => new { e.VehicleId, e.IsCurrent })
                .IsUnique()
                .HasFilter("IsCurrent = 1");

            entity.HasOne(e => e.ServiceBooking)
                .WithOne(s => s.ServiceEvent)
                .HasForeignKey<ServiceEvent>(e => new { e.CompanyCode, e.VehicleId, e.ServiceBookingId })
                .HasPrincipalKey<ServiceBooking>(v => new { v.CompanyCode, v.VehicleId, v.Id })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Vehicle)
                .WithMany(v => v.ServiceEvents)
                .HasForeignKey(e => new { e.CompanyCode, e.VehicleId })
                .HasPrincipalKey(v => new { v.CompanyCode, v.Id })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}