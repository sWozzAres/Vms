namespace Vms.Domain.ServiceBookingProcess
{
    [Table("ServiceBookingLocks")]
    public class ServiceBookingLock
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ServiceBookingId { get; set; }
        public ServiceBooking ServiceBooking { get; private set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; private set; } = null!;

        [StringLength(User.UserName_MaxLength)]
        public string UserName { get; set; } = null!;

        public DateTime Granted { get; set; }

        public static ServiceBookingLock Create(Guid serviceBookingId, string userId, string userName,
            DateTime granted)
        => new()
        {
            Id = Guid.NewGuid(),
            ServiceBookingId = serviceBookingId,
            UserId = userId,
            UserName = userName,
            Granted = granted,
        };
    }
}
namespace Vms.Domain.ServiceBookingProcess.Configuration
{
    public class ServiceBookingLockEntityTypeConfiguration : IEntityTypeConfiguration<ServiceBookingLock>
    {
        public void Configure(EntityTypeBuilder<ServiceBookingLock> entity)
        {
            entity.HasOne(e => e.ServiceBooking)
                .WithOne(e => e.Lock)
                .HasForeignKey<ServiceBookingLock>(e => e.ServiceBookingId);

            entity.HasOne(e => e.User)
                .WithMany();
        }
    }
}