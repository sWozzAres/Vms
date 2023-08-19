namespace Utopia.Api.Domain.System
{
    [Table("Users", Schema = "System")]
    public class User(string userId, string userName, string tenantId, string emailAddress)
    {
        public const int UserId_MaxLength = 50;
        public const int UserName_MaxLength = 50;

        [Key]
        [StringLength(UserId_MaxLength)]
        public string UserId { get; private set; } = userId;

        [StringLength(50)]
        public string UserName { get; set; } = userName;

        [StringLength(10)]
        public string TenantId { get; set; } = tenantId;

        [StringLength(128)]
        public string EmailAddress { get; set; } = emailAddress;
    }
}

namespace Utopia.Api.Domain.System.Configuration
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
        }
    }
}