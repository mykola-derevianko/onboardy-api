using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired();
            
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.EmailVerified)
                .HasDefaultValue(false);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'")
                .IsRequired();

            builder.Property(u => u.ProfilePictureBlobName)
                .HasMaxLength(500)
                .IsRequired(false);

        }
    }
}
