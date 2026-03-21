using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Role)
                .IsRequired();

            builder.Property(m => m.Status)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Organization)
                .WithMany()
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => new { m.UserId, m.OrganizationId })
                .IsUnique();

            builder.HasIndex(m => m.OrganizationId);
        }
    }
}