﻿namespace Vms.Domain.System
{
    public class Follower(Guid documentId, string userId, string emailAddress)
    {
        public long Id { get; private set; }
        public Guid DocumentId { get; set; } = documentId;
        public string UserId { get; set; } = userId;
        public string EmailAddress { get; set; } = emailAddress;
    }
}
namespace Vms.Domain.System.Configuration
{
    public class FollowerEntityTypeConfiguration : IEntityTypeConfiguration<Follower>
    {
        public void Configure(EntityTypeBuilder<Follower> entity)
        {
            entity.ToTable("Followers", "System");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DocumentId, e.UserId }).IsUnique();
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(128);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId);
        }
    }
}