namespace Vms.Domain.ServiceBookingProcess
{
    public class ServiceBookingLock
    {
        public Guid Id { get; set; }
        public Guid ServiceBookingId { get; set; }
        public ServiceBooking ServiceBooking { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime Granted { get; set; }
        public static ServiceBookingLock Create(Guid serviceBookingId, string userId, string userName, DateTime granted)
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
            entity.ToTable("ServiceBookingLocks");

            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.ServiceBooking)
                .WithOne(e => e.Lock)
                .HasForeignKey<ServiceBookingLock>(e => e.ServiceBookingId);
        }
    }
}