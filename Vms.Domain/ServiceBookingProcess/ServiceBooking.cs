using Vms.Domain.Core;
using Vms.Domain.System;

namespace Vms.Domain.ServiceBookingProcess
{
    public enum ServiceBookingStatus : int
    {
        Cancelled = -2,
        Complete = -1,
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
        public string Ref { get; set; } = null!;
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public DateOnly? PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? MotDue { get; set; }
        public string? SupplierCode { get; private set; }
        public Supplier? Supplier { get; private set; }
        public DateOnly? BookedDate { get; private set; }
        public DateTime? EstimatedCompletion { get; set; }
        //public VehicleMot? VehicleMot { get; set; }
        //public ServiceBookingSupplier? Supplier { get; private set; }
        public DateTime? RescheduleTime { get; internal set; }
        public ServiceBookingStatus Status { get; private set; }
        public ServiceLevel ServiceLevel { get; set; }
        public Guid? MotEventId { get; set; }
        public MotEvent? MotEvent { get; set; }
        public Guid? LockId { get; set; }
        public ServiceBookingLock? Lock { get; set; }
        public ServiceBookingDriver Driver { get; private set; } = new();
        public ServiceBookingContact Contact { get; set; } = new();
        public string CreatedUserId { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public string? AssignedToUserId { get; set; }
        public User? AssignedTo { get; set; }
        public string? OwnerUserId { get; set; }
        public User? Owner { get; set; }

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

        public void SetDriver(string name, string emailAddress, string mobileNumber)
        {
            Driver.Name = name;
            Driver.EmailAddress = emailAddress;
            Driver.MobileNumber = mobileNumber;
        }
        public bool IsValid
            => (PreferredDate1 is not null || PreferredDate2 is not null || PreferredDate3 is not null) && ServiceLevel != ServiceLevel.None;

        public void ChangeStatus(ServiceBookingStatus status, DateTime? rescheduleTime = null)
        {
            Status = status;
            RescheduleTime = rescheduleTime;
        }
        public void Assign(string supplierCode)
        {
            SupplierCode = supplierCode;
            ChangeStatus(ServiceBookingStatus.Book, DateTime.Now);
        }
        public void Book(DateOnly bookedDate)
        {
            BookedDate = bookedDate;
            ChangeStatus(ServiceBookingStatus.Confirm, DateTime.Now);
        }
        public void Unbook()
        {
            BookedDate = null;
            ChangeStatus(ServiceBookingStatus.Book, DateTime.Now);
        }
        public void Unassign()
        {
            SupplierCode = null;
            ChangeStatus(ServiceBookingStatus.Assign, DateTime.Now);
        }
    }

    public class ServiceBookingDriver
    {
        public const int Name_MaxLength = Driver.FirstName_MaxLength + Driver.LastName_MaxLength + 1;
        public const int Email_MaxLength = Driver.Email_MaxLength;
        public const int MobileNumber_MaxLength = Driver.MobileNumber_MaxLength;
        public Guid ServiceBookingId { get; set; }
        public string? Name { get; internal set; }
        public string? EmailAddress { get; internal set; }
        public string? MobileNumber { get; internal set; }
        public ServiceBooking ServiceBooking { get; set; } = null!;
    }

    public class ServiceBookingContact
    {
        public const int Name_MaxLength = Driver.FirstName_MaxLength + Driver.LastName_MaxLength + 1;
        public const int Email_MaxLength = Driver.Email_MaxLength;
        public const int MobileNumber_MaxLength = Driver.MobileNumber_MaxLength;
        public Guid ServiceBookingId { get; set; }
        public string? Name { get; set; }
        public string? EmailAddress { get; set; }
        public string? MobileNumber { get; set; }
        public ServiceBooking ServiceBooking { get; set; } = null!;
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.ToTable("ServiceBookings");
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Ref).IsUnique().HasDatabaseName("UQ_ServiceBooking_Ref");

            builder.Property(e => e.CompanyCode)
                .HasMaxLength(Company.Code_MaxLength)
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
                .HasConstraintName("FK_ServiceBookings_Vehicles");

            builder.OwnsOne(e => e.Driver, d =>
            {
                d.ToTable("ServiceBookingDrivers", tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("ServiceBookingDriversHistory");
                    ttb
                        .HasPeriodStart("ValidFrom")
                        .HasColumnName("ValidFrom");
                    ttb
                        .HasPeriodEnd("ValidTo")
                        .HasColumnName("ValidTo");
                }));

                d.Property(e => e.Name)
                    .HasMaxLength(ServiceBookingDriver.Name_MaxLength);
                d.Property(e => e.EmailAddress)
                    .HasMaxLength(ServiceBookingDriver.Email_MaxLength);
                d.Property(e => e.MobileNumber)
                    .HasMaxLength(ServiceBookingDriver.MobileNumber_MaxLength);

                d.WithOwner(x => x.ServiceBooking)
                    .HasForeignKey(d => d.ServiceBookingId)
                    .HasConstraintName("FK_ServiceBookings_ServiceBookingDrivers");
            });

            builder.OwnsOne(e => e.Contact, d =>
            {
                d.ToTable("ServiceBookingContacts", tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("ServiceBookingContactsHistory");
                    ttb
                        .HasPeriodStart("ValidFrom")
                        .HasColumnName("ValidFrom");
                    ttb
                        .HasPeriodEnd("ValidTo")
                        .HasColumnName("ValidTo");
                }));

                d.Property(e => e.Name)
                    .HasMaxLength(ServiceBookingContact.Name_MaxLength);
                d.Property(e => e.EmailAddress)
                    .HasMaxLength(ServiceBookingContact.Email_MaxLength);
                d.Property(e => e.MobileNumber)
                    .HasMaxLength(ServiceBookingContact.MobileNumber_MaxLength);

                d.WithOwner(x => x.ServiceBooking)
                    .HasForeignKey(d => d.ServiceBookingId)
                    .HasConstraintName("FK_ServiceBookings_ServiceBookingContacts");
            });

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