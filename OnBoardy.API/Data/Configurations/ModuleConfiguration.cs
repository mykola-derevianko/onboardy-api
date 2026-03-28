using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.BannerBlobName)
                .HasMaxLength(500);

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.HasOne(m => m.CreatedByUser)
                .WithMany()
                .HasForeignKey(m => m.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Organization)
                .WithMany()
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.OrganizationId);
        }
    }
}
