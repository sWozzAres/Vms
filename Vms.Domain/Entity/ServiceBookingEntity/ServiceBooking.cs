﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Exceptions;

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public enum ServiceBookingStatus : int
    {
        None = 0,
        Assign = 1,
        Book = 2,
        Confirm = 3,
        CheckArrival = 4,
        CheckWorkStatus = 5,
        ChaseDriver = 6,
        RebookDriver = 7
    };

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
        public ServiceBookingStatus Status { get; private set; }
        public Vehicle Vehicle { get; set; } = null!;
        public Guid? MotEventId { get; set; }
        public MotEvent? MotEvent { get; set; }
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

            //TODO remove
            MotDue = motDue;

            Status = IsValid
                ? ServiceBookingStatus.Assign
                : ServiceBookingStatus.None;
        }
        public bool IsValid
            => PreferredDate1 is not null || PreferredDate2 is not null || PreferredDate3 is not null;

        public void ChangeStatus(ServiceBookingStatus status) => Status = status;
        public void Assign(string supplierCode)
        {
            SupplierCode = supplierCode;
            Status = ServiceBookingStatus.Book;
            //Supplier = new ServiceBookingSupplier(Id, supplierCode);
        }
        //public void UnassignSupplier()
        //{
        //    SupplierCode = null;
        //    //Supplier = new ServiceBookingSupplier(Id, supplierCode);
        //}
        public void Unbook()
        {
            BookedDate = null;
            RescheduleTime = DateTime.Now;
            ChangeStatus(ServiceBookingStatus.Book);
        }
        public void Book(DateOnly bookedDate)
        {
            if (SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            RescheduleTime = null;
            BookedDate = bookedDate;
            ChangeStatus(ServiceBookingStatus.Confirm);
        }
        public void UnassignSupplier()
        {
            RescheduleTime = null;
            SupplierCode = null;
            ChangeStatus(ServiceBookingStatus.Assign);
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