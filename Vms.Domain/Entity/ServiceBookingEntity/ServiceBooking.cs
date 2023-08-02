using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Exceptions;

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public enum ServiceBookingStatus : int
    {
        Cancelled = -1,
        None = 0,
        Assign = 1,
        Book = 2,
        Confirm = 3,
        CheckArrival = 4,
        CheckWorkStatus = 5,
        ChaseDriver = 6,
        RebookDriver = 7,
        NotifyCustomer = 8,
        NotifyCustomerDelay = 9
    };

    public enum ServiceLevel : int
    {
        None = 0,
        Mobile = 1,
        Collection = 2,
        DropOff = 3,
        WhileYouWait = 4,
        DropOffWithCourtesyCar = 5,
    };

    public class ServiceBooking
    {
        public string CompanyCode { get; set; } = null!;
        public Guid Id { get; private set; }
        public string Ref { get; set; }
        public Guid VehicleId { get; set; }
        public DateOnly? PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? MotDue { get; set; }
        public string? SupplierCode { get; internal set; }
        public Supplier? Supplier { get; private set; }
        public DateOnly? BookedDate { get; internal set; }
        public DateTime? EstimatedCompletion { get; set; }
        //public VehicleMot? VehicleMot { get; set; }
        //public ServiceBookingSupplier? Supplier { get; private set; }
        public DateTime? RescheduleTime { get; internal set; }
        public ServiceBookingStatus Status { get; private set; }
        public ServiceLevel ServiceLevel { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public Guid? MotEventId { get; set; }
        public MotEvent? MotEvent { get; set; }
        public ServiceBookingLock? Lock { get; set; }

        public string CreatedUserId { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public string? AssignedToUserId { get; set; }
        public User? AssignedTo { get; set; }
        public string? OwnerUserId { get; set; }
        public User? Owner { get; set; }

        public Guid? LockId { get; set; }
        private ServiceBooking() { }
        public ServiceBooking(string companyCode, Guid vehicleId,
            DateOnly? preferredDate1, DateOnly? preferredDate2, DateOnly? preferredDate3, DateOnly? motDue,
            string createdUserId)
        {
            CompanyCode = companyCode;
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            PreferredDate1 = preferredDate1;
            PreferredDate2 = preferredDate2;
            PreferredDate3 = preferredDate3;

            //TODO remove
            MotDue = motDue;
            CreatedUserId = createdUserId;
            Status = IsValid
                ? ServiceBookingStatus.Assign
                : ServiceBookingStatus.None;

            var r = (uint)new Random().NextInt64(1111111111, uint.MaxValue);
            Ref = r.ToString()[..3] + "-" + r.ToString()[3..6] + "-" + r.ToString()[6..10];
        }

        public bool IsValid
            => (PreferredDate1 is not null || PreferredDate2 is not null || PreferredDate3 is not null) && ServiceLevel != ServiceLevel.None;

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

            //RescheduleTime = null;
            BookedDate = bookedDate;
            ChangeStatus(ServiceBookingStatus.Confirm);
        }
        public void Unassign()
        {
            //RescheduleTime = null;
            SupplierCode = null;
            ChangeStatus(ServiceBookingStatus.Assign);
        }
        //public void Reschedule(DateTime? rescheduleTime) => RescheduleTime = rescheduleTime;
    }

    public class ServiceBookingLock
    {
        public Guid Id { get; set; }
        public Guid ServiceBookingId { get; set; }
        public ServiceBooking ServiceBooking { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime Granted { get; set; }
        public static ServiceBookingLock Create(Guid serviceBookingId, string userId, string userName)
        => new()
        {
            Id = Guid.NewGuid(),
            ServiceBookingId = serviceBookingId,
            UserId = userId,
            UserName = userName,
            Granted = DateTime.Now,
        };
    }

    public class Follower
    {
        public Guid DocumentId { get; set; }
        public string UserId { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
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
    public class FollowerEntityTypeConfiguration : IEntityTypeConfiguration<Follower>
    {
        public void Configure(EntityTypeBuilder<Follower> entity)
        {
            entity.ToTable("Followers");

            entity.HasKey(e=>new {e.DocumentId, e.UserId});
        }
    }
    public class ServiceBookingLockEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBookingLock>
    {
        public void Configure(EntityTypeBuilder<ServiceBookingLock> entity)
        {
            entity.ToTable("ServiceBookingLock");

            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.ServiceBooking)
                .WithOne(e => e.Lock)
                .HasForeignKey<ServiceBookingLock>(e=>e.ServiceBookingId);
        }
    }

    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.ToTable("ServiceBooking");
            builder.HasKey(e => e.Id);

            builder.HasIndex(e=>e.Ref).IsUnique().HasDatabaseName("UQ_ServiceBooking_Ref");

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(10)
                .IsFixedLength();

            builder.Property(e => e.SupplierCode).HasMaxLength(8);
            builder.HasOne(e => e.Supplier)
                .WithMany();

            builder.Property(e => e.AssignedToUserId)
                .HasMaxLength(User.UserId_MaxLength);
            builder.HasOne(e => e.AssignedTo)
                .WithMany()
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.OwnerUserId)
                .HasMaxLength(User.UserId_MaxLength);
            builder.HasOne(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(e => e.CreatedUserId)
                .HasMaxLength(User.UserId_MaxLength);
            builder.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

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