namespace Utopia.Api.Domain.System
{
    public class Login
    {
        public long Id { get; private set; }
        public string UserId { get; private set; } = null!;
        //public User User { get; private set; } = null!;
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
            entity.ToTable("Logins", "System");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.UserId)
                .HasMaxLength(User.UserId_MaxLength);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}