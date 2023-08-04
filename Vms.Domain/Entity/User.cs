using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vms.Domain.Entity
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

    public class Login
    {
        public long Id { get; private set; }
        public string UserId { get; private set; } = null!;
        public User User { get; private set; } = null!;
        public DateTime LoginTime { get; private set; }
        public Login(string userId, DateTime loginTime)
            => (UserId, LoginTime) = (userId, loginTime);
    }
}

namespace Vms.Domain.Entity.Configuration
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("Users", "ClientApp");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId)
                .HasMaxLength(User.UserId_MaxLength);
            entity.Property(e => e.TenantId)
                .HasMaxLength(Company.Code_MaxLength);
            entity.Property(e => e.UserName)
                .HasMaxLength(50);
        }
    }

    public class LoginEntityTypeConfiguration : IEntityTypeConfiguration<Login>
    {
        public void Configure(EntityTypeBuilder<Login> entity)
        {
            entity.ToTable("Logins", "ClientApp");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.UserId)
                .HasMaxLength(User.UserId_MaxLength);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}