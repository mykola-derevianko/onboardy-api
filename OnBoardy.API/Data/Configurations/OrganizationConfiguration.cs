using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OwnerId);

            builder.HasIndex(o => o.OwnerId)
                .IsUnique();

            builder.Property(o => o.Name)
                .IsRequired();

            builder.Property(o => o.Description);

            builder.Property(o => o.CreatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'")
                .IsRequired();

            builder.HasOne(o => o.Owner)
                .WithOne(u => u.OwnedOrganization)
                .HasForeignKey<Organization>(o => o.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
