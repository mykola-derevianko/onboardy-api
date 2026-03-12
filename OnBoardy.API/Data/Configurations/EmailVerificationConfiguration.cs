using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
    {
        public void Configure(EntityTypeBuilder<EmailVerification> builder)
        {
            builder.HasKey(ev => ev.Id);

            builder.Property(ev => ev.Token)
                .IsRequired();

            builder.HasIndex(ev => ev.Token)
                .IsUnique();

            builder.Property(ev => ev.ExpiresAt)
                .IsRequired();
            builder.Property(ev => ev.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasOne(ev => ev.User)
                .WithMany(u => u.EmailVerifications)
                .HasForeignKey(ev => ev.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
