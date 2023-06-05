using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
{
    public enum ServiceBookingStatus { None, Assigned };

    public class ServiceBooking
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; set; }
        public DateOnly PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? MotDue { get; set; }
        public ServiceBookingSupplier? Supplier { get; private set; }

        public ServiceBookingStatus Status { get; private set; }
        public virtual Vehicle Vehicle { get; set; } = null!;
        private ServiceBooking() { }
        public ServiceBooking(Guid vehicleId, DateOnly preferredDate1, DateOnly? preferredDate2, DateOnly? preferredDate3, DateOnly? motDue)
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            PreferredDate1 = preferredDate1;
            PreferredDate2 = preferredDate2;
            PreferredDate3 = preferredDate3;
            MotDue = motDue;
            Status = ServiceBookingStatus.None;
        }
        public void ChangeStatus(ServiceBookingStatus status) => Status = status;
        public void AssignSupplier(string supplierCode)
        {
            Supplier = new ServiceBookingSupplier(Id, supplierCode);
        }
    }

    public class ServiceBookingSupplier
    {
        public Guid ServiceBookingId { get; private set; }
        public string SupplierCode { get; private set; } = null!;
        public virtual ServiceBooking ServiceBooking { get; private set; } = null!;
        public virtual Supplier Supplier { get; private set; } = null!;
        private ServiceBookingSupplier() { }
        public ServiceBookingSupplier(Guid serviceBookingId, string supplierCode) 
            => (ServiceBookingId, SupplierCode) = (serviceBookingId, supplierCode);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.ToTable("ServiceBooking");
            builder.HasKey(e => e.Id);

            builder.OwnsOne(d => d.Supplier, x =>
            {
                x.ToTable("ServiceBookingSupplier", tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("ServiceBookingSupplierHistory");
                    ttb
                        .HasPeriodStart("ValidFrom")
                        .HasColumnName("ValidFrom");
                    ttb
                        .HasPeriodEnd("ValidTo")
                        .HasColumnName("ValidTo");
                }));

                x.WithOwner(d => d.ServiceBooking)
                    .HasForeignKey(d => d.ServiceBookingId);

                x.HasOne(d => d.Supplier).WithMany().HasForeignKey(d => d.SupplierCode);
            });
        }
    }
}