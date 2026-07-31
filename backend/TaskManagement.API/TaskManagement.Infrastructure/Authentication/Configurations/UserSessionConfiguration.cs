using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Authentication.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceName)
            .HasMaxLength(100);

        builder.Property(x => x.Browser)
            .HasMaxLength(100);

        builder.Property(x => x.OperatingSystem)
            .HasMaxLength(100);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(100);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.UserSessions)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RefreshToken)
            .WithMany(x => x.UserSessions)
            .HasForeignKey(x => x.RefreshTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}