using Vms.Domain.Core;

namespace Vms.Domain.System
{
    public class User
    {
        public const int UserId_MaxLength = 50;
        public string UserId { get; private set; } = null!;
        public string UserName { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public User(string userId, string userName, string tenantId)
            => (UserId, UserName, TenantId) = (userId, userName, tenantId);
    }
}

namespace Vms.Domain.System.Configuration
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users", "System");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                .HasMaxLength(User.UserId_MaxLength);
            entity.Property(e => e.TenantId)
                .HasMaxLength(Company.Code_MaxLength);
            entity.Property(e => e.UserName)
                .HasMaxLength(50);
        }
    }
}