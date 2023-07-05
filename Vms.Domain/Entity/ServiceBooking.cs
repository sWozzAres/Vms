﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vms.Domain.Exceptions;

namespace Vms.Domain.Entity
{
    public enum ServiceBookingStatus { None, Assign, Book, Confirm };

    public class ServiceBooking
    {
        public string CompanyCode { get; set; } = null!;
        public Guid Id { get; private set; }
        public Guid VehicleId { get; set; }
        public DateOnly? PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? MotDue { get; set; }
        public string? SupplierCode { get; internal set; }
        public Supplier? Supplier { get; private set; }
        public DateOnly? BookedDate { get; internal set; }
        //public VehicleMot? VehicleMot { get; set; }
        //public ServiceBookingSupplier? Supplier { get; private set; }
        public DateTime? RescheduleTime { get; internal set; }
        public ServiceBookingStatus Status { get; internal set; }
        public Vehicle Vehicle { get; set; } = null!;
        private ServiceBooking() { }
        public ServiceBooking(string companyCode, Guid vehicleId,
            DateOnly? preferredDate1, DateOnly? preferredDate2, DateOnly? preferredDate3, DateOnly? motDue)
        {
            CompanyCode = companyCode;
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            PreferredDate1 = preferredDate1;
            PreferredDate2 = preferredDate2;
            PreferredDate3 = preferredDate3;
            MotDue = motDue;
            Status = ServiceBookingStatus.None;
        }
        //public void ChangeStatus(ServiceBookingStatus status) => Status = status;
        //public void AssignSupplier(string supplierCode)
        //{
        //    SupplierCode = supplierCode;
        //    //Supplier = new ServiceBookingSupplier(Id, supplierCode);
        //}
        //public void UnassignSupplier()
        //{
        //    SupplierCode = null;
        //    //Supplier = new ServiceBookingSupplier(Id, supplierCode);
        //}
        public void Book(DateOnly bookedDate)
        {
            if (SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            RescheduleTime = null;
            BookedDate = bookedDate;
            Status = ServiceBookingStatus.Confirm;
        }
        //public void Reschedule(DateTime? rescheduleTime) => RescheduleTime = rescheduleTime;
    }

    //public class ServiceBookingSupplier
    //{
    //    public Guid ServiceBookingId { get; private set; }
    //    public string? SupplierCode { get; private set; } = null!;
    //    public DateOnly? BookedDate { get; set; }
        
    //    public virtual ServiceBooking ServiceBooking { get; private set; } = null!;
    //    public virtual Supplier Supplier { get; private set; } = null!;
    //    private ServiceBookingSupplier() { }
    //    public ServiceBookingSupplier(Guid serviceBookingId, string supplierCode)
    //        => (ServiceBookingId, SupplierCode) = (serviceBookingId, supplierCode);
    //}

    //public class ServiceBookingRefusal
    //{
    //    public string CompanyCode { get; set; } = null!;
    //    public Guid ServiceBookingId { get; set; }
    //    public ServiceBooking ServiceBooking { get; set; } = null!;
    //    public string SupplierCode { get; set; } = null!;
    //    public Supplier Supplier { get; set; } = null!;
    //    public string RefusalReasonCode { get; set; } = null!;
    //    public RefusalReason RefusalReason { get; set; } = null!;
    //}
}

namespace Vms.Domain.Entity.Configuration
{
    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.ToTable("ServiceBooking");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.Property(e => e.SupplierCode).HasMaxLength(8);
            builder.HasOne(e => e.Supplier)
                .WithMany();

            builder.HasOne(e => e.Vehicle)
                .WithMany(v => v.ServiceBookings)
                .HasForeignKey(e => new { e.CompanyCode, e.VehicleId })
                .HasPrincipalKey(v => new { v.CompanyCode, v.Id })
                .HasConstraintName("FK_ServiceBookings_Vehicle");

            //builder.HasOne(e => e.VehicleMot)
            //    .WithOne(m => m.ServiceBooking)
            //    .HasForeignKey<ServiceBooking>(e => new { e.VehicleId, e.MotDue })
            //    .HasPrincipalKey<VehicleMot>(m => new { m.VehicleId, m.Due })
            //    .IsRequired(false);

            //builder.OwnsOne(d => d.Supplier, x =>
            //{
            //    x.ToTable("ServiceBookingSupplier", tb => tb.IsTemporal(ttb =>
            //    {
            //        ttb.UseHistoryTable("ServiceBookingSupplierHistory");
            //        ttb
            //            .HasPeriodStart("ValidFrom")
            //            .HasColumnName("ValidFrom");
            //        ttb
            //            .HasPeriodEnd("ValidTo")
            //            .HasColumnName("ValidTo");
            //    }));

            //    x.Property(s => s.RefusalReasonCode).HasMaxLength(10);
            //    x.Property(s => s.RefusalReasonName).HasMaxLength(32);

            //    x.WithOwner(d => d.ServiceBooking)
            //        .HasForeignKey(d => d.ServiceBookingId);

            //    x.HasOne(d => d.Supplier).WithMany().HasForeignKey(d => d.SupplierCode);
            //});
        }
    }
}