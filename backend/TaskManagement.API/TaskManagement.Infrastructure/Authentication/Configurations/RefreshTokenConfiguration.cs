using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Authentication.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CreatedByIp)
            .HasMaxLength(100);

        builder.Property(x => x.RevokedByIp)
            .HasMaxLength(100);

        builder.Property(x => x.ReasonRevoked)
            .HasMaxLength(300);

        builder.Property(x => x.ReplacedByTokenHash)
            .HasMaxLength(500);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.ApplicationUserId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}