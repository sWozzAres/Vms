namespace Utopia.Api.Domain.System
{
    public class User(string userId, string userName, string tenantId, string emailAddress)
    {
        public const int UserId_MaxLength = 50;
        public string UserId { get; private set; } = userId;
        public string UserName { get; set; } = userName;
        public string TenantId { get; set; } = tenantId;
        public string EmailAddress { get; set; } = emailAddress;
    }
}

namespace Utopia.Api.Domain.System.Configuration
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
                .HasMaxLength(10);
            entity.Property(e => e.UserName)
                .HasMaxLength(50);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(128);
        }
    }
}