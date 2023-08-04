using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vms.Domain.Entity.ServiceBookingEntity;

namespace Vms.Domain.Entity.ServiceBookingEntity
{
    public class Follower(Guid documentId, string userId, string emailAddress)
    {
        public Guid DocumentId { get; set; } = documentId;
        public string UserId { get; set; } = userId;
        public string EmailAddress { get; set; } = emailAddress;
    }
}
namespace Vms.Domain.Entity.Configuration
{
    public class FollowerEntityTypeConfiguration : IEntityTypeConfiguration<Follower>
    {
        public void Configure(EntityTypeBuilder<Follower> entity)
        {
            entity.ToTable("Followers");

            entity.HasKey(e => new { e.DocumentId, e.UserId });

            entity.Property(e => e.EmailAddress)
                .HasMaxLength(128);
        }
    }
}