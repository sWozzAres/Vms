using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public int VehicleId { get; set; }
        public DateOnly PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? MotDue { get; set; }
        public int? SupplierId { get; set; }

        public ServiceBookingStatus Status { get; private set; } = ServiceBookingStatus.None;
        public virtual Vehicle Vehicle { get; set; } = null!;
        private ServiceBooking() { }
        public ServiceBooking(int vehicleId, DateOnly preferredDate1, DateOnly? preferredDate2, DateOnly? preferredDate3, DateOnly? motDue)
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
}

namespace Vms.Domain.Entity.Configuration
{
    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.ToTable("ServiceBooking");
        }
    }
}