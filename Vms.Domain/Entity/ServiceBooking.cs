using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Domain.Entity
{
    public enum ServiceBookingStatus { None, Allocating };

    public class ServiceBooking
    {
        public int Id { get; private set; }
        public Guid VehicleId { get; set; }
        public DateOnly PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? MotDue { get; set; }
        public Geometry VehicleLocation { get; set; } = null!;
        public ServiceBookingSupplier? Supplier { get; set; }

        public ServiceBookingStatus Status { get; private set; } = ServiceBookingStatus.None;
        public virtual Vehicle Vehicle { get; set; } = null!;
        private ServiceBooking() { }
        public ServiceBooking(Guid vehicleId, DateOnly preferredDate1, DateOnly? preferredDate2, DateOnly? preferredDate3, DateOnly? motDue)
        {
            VehicleId = vehicleId;
            PreferredDate1 = preferredDate1;
            PreferredDate2 = preferredDate2;
            PreferredDate3 = preferredDate3;
            MotDue = motDue;
            Status = ServiceBookingStatus.Allocating;
        }
        public void ChangeStatus(ServiceBookingStatus status) => Status = status;
    }

    public class ServiceBookingSupplier
    {
        public int ServiceBookingId { get; set; }
        public string SupplierCode { get; set; } = null!;
        public virtual ServiceBooking ServiceBooking { get; set; } = null!;
        public virtual Supplier Supplier { get; set; } = null!;
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.ToTable("ServiceBooking");

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