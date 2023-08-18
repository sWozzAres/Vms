using System.ComponentModel.DataAnnotations.Schema;

namespace Utopia.Api.Domain.System
{
    [Table("Logins", Schema = "System")]
    public class Login
    {
        [Key]
        public long Id { get; private set; }

        public string UserId { get; private set; } = null!;
        public User User { get; private set; } = null!;

        public DateTime LoginTime { get; private set; }

        public Login(string userId, DateTime loginTime)
            => (UserId, LoginTime) = (userId, loginTime);
    }
}
namespace Utopia.Api.Domain.System.Configuration
{
    public class LoginEntityTypeConfiguration : IEntityTypeConfiguration<Login>
    {
        public void Configure(EntityTypeBuilder<Login> entity)
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}