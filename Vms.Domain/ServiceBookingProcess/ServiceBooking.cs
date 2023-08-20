using Vms.Domain.Core;

namespace Vms.Domain.ServiceBookingProcess
{
    [Table("ServiceBookings")]
    public class ServiceBooking
    {
        public string CompanyCode { get; set; } = null!;

        [Key]
        public Guid Id { get; private set; }

        /// <summary>
        /// Unique reference number
        /// </summary>
        public string Ref { get; set; } = null!;

        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; private set; } = null!;

        public DateOnly? PreferredDate1 { get; set; }
        public DateOnly? PreferredDate2 { get; set; }
        public DateOnly? PreferredDate3 { get; set; }
        public DateOnly? BookedDate { get; private set; }
        public DateTime? EstimatedCompletion { get; set; }
        public DateTime? RescheduleTime { get; internal set; }
        public ServiceBookingStatus Status { get; private set; }
        public ServiceLevel ServiceLevel { get; set; }

        public string? SupplierCode { get; private set; }
        public Supplier? Supplier { get; private set; }

        public Guid? MotEventId { get; set; }
        public MotEvent? MotEvent { get; private set; }

        public Guid? ServiceEventId { get; set; }
        public ServiceEvent? ServiceEvent { get; private set; }

        public Guid? LockId { get; set; }
        public ServiceBookingLock? Lock { get; private set; }

        public ServiceBookingDriver Driver { get; private set; } = new();
        public ServiceBookingContact Contact { get; private set; } = new();

        public string CreatedUserId { get; set; } = null!;
        public User CreatedBy { get; private set; } = null!;

        public string? AssignedToUserId { get; set; }
        public User? AssignedTo { get; private set; }

        public string? OwnerUserId { get; set; }
        public User? Owner { get; private set; }

        private ServiceBooking() { }
        public ServiceBooking(string companyCode, Guid vehicleId,
            DateOnly? preferredDate1, DateOnly? preferredDate2, DateOnly? preferredDate3,
            ServiceLevel serviceLevel,
            string createdUserId,
            Driver? driver = null,
            MotEvent? motEvent = null)
        {
            CompanyCode = companyCode;
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            PreferredDate1 = preferredDate1;
            PreferredDate2 = preferredDate2;
            PreferredDate3 = preferredDate3;
            ServiceLevel = serviceLevel;

            if (driver is not null)
                AddDriver(driver);

            if (motEvent is not null)
                AddMotEvent(motEvent);
            
            CreatedUserId = createdUserId;
            Status = ServiceBookingStatus.None; 
            Ref = GenerateRandomRef();
        }
        public void AddMotEvent(MotEvent motEvent)
        {
            if (motEvent.VehicleId != VehicleId)
                throw new InvalidOperationException("MotEvent is not for this vehicle.");

            if (motEvent.ServiceBookingId is not null)
                throw new VmsDomainException("Mot Event is already assigned to a service booking.");

            if (!motEvent.IsCurrent)
                throw new VmsDomainException("Mot Event is not current.");

            motEvent.AddToServiceBooking(this);
            MotEvent = motEvent;
            MotEventId = motEvent.Id;
        }
        public void RemoveMotEvent()
        {
            if (MotEvent is null)
                throw new InvalidOperationException("MotEvent is null");

            MotEvent.RemoveFromServiceBooking();
            MotEvent = null;
            MotEventId = null;
        }
        /// <summary>
        /// Generates a random 10 digit number.
        /// </summary>
        private static string GenerateRandomRef()
        {
            var r = (uint)new Random().NextInt64(1111111111, uint.MaxValue);
            return r.ToString()[..3] + "-" + r.ToString()[3..6] + "-" + r.ToString()[6..10];
        }
        public void AddDriver(Driver driver)
        {
            Driver.Name = string.Join(" ", driver.FirstName, driver.LastName);
            Driver.EmailAddress = driver.EmailAddress;
            Driver.MobileNumber = driver.MobileNumber;
        }
        public bool IsReady
            => (PreferredDate1 is not null || PreferredDate2 is not null || PreferredDate3 is not null) && ServiceLevel != ServiceLevel.None;

        public void ChangeStatus(ServiceBookingStatus status, DateTime? rescheduleTime = null)
        {
            Status = status;
            RescheduleTime = rescheduleTime;
        }
        public void Assign(string supplierCode)
            => SupplierCode = supplierCode;
        public void Book(DateOnly bookedDate)
            => BookedDate = bookedDate;
        public void Unbook()
            => BookedDate = null;
        public void Unassign()
            => SupplierCode = null;
    }

    public class ServiceBookingDriver
    {
        public const int Name_MaxLength = Driver.FirstName_MaxLength + Driver.LastName_MaxLength + 1;
        public const int Email_MaxLength = Driver.Email_MaxLength;
        public const int MobileNumber_MaxLength = Driver.MobileNumber_MaxLength;

        public Guid ServiceBookingId { get; set; }
        public ServiceBooking ServiceBooking { get; private set; } = null!;

        [StringLength(Name_MaxLength)]
        public string? Name { get; internal set; }

        [StringLength(Email_MaxLength)]
        public string? EmailAddress { get; internal set; }

        [StringLength(MobileNumber_MaxLength)]
        public string? MobileNumber { get; internal set; }
    }

    public class ServiceBookingContact
    {
        public const int Name_MaxLength = Driver.FirstName_MaxLength + Driver.LastName_MaxLength + 1;
        public const int Email_MaxLength = Driver.Email_MaxLength;
        public const int MobileNumber_MaxLength = Driver.MobileNumber_MaxLength;

        public Guid ServiceBookingId { get; set; }
        public ServiceBooking ServiceBooking { get; private set; } = null!;

        [StringLength(Name_MaxLength)]
        public string? Name { get; internal set; }

        [StringLength(Email_MaxLength)]
        public string? EmailAddress { get; internal set; }

        [StringLength(MobileNumber_MaxLength)]
        public string? MobileNumber { get; internal set; }
    }
}

namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class ServiceBookingEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            builder.HasIndex(e => e.Ref)
                .IsUnique()
                .HasDatabaseName("UQ_ServiceBooking_Ref");

            builder.HasOne(e => e.Vehicle)
                .WithMany(v => v.ServiceBookings)
                .HasForeignKey(e => new { e.CompanyCode, e.VehicleId })
                .HasPrincipalKey(v => new { v.CompanyCode, v.Id })
                .HasConstraintName("FK_ServiceBookings_Vehicles");

            builder.HasOne(e => e.Supplier)
                .WithMany();

            builder.HasOne(e => e.AssignedTo)
                .WithMany()
                .HasForeignKey(e => e.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

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

                d.WithOwner(x => x.ServiceBooking)
                    .HasForeignKey(d => d.ServiceBookingId)
                    .HasConstraintName("FK_ServiceBookings_ServiceBookingContacts");
            });
        }
    }
}