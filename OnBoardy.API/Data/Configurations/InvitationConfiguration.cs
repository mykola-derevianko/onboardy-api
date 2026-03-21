using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Email)
                .HasMaxLength(255);

            builder.Property(i => i.Role)
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<Enums.MembershipRole>(v, true))
                .IsRequired();

            builder.Property(i => i.Status)
                .HasConversion(
                    v => v.ToString().ToLowerInvariant(),
                    v => Enum.Parse<Enums.InvitationStatus>(v, true))
                .IsRequired();

            builder.Property(i => i.AssignedModules)
                .HasColumnType("jsonb");

            builder.Property(i => i.Token)
                .IsRequired();

            builder.HasIndex(i => i.Token)
                .IsUnique();

            builder.Property(i => i.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasOne(i => i.Organization)
                .WithMany()
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.InvitedByUser)
                .WithMany()
                .HasForeignKey(i => i.InvitedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}