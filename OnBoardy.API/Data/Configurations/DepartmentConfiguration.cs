using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.OrganizationId)
                .IsRequired();

            builder.Property(d => d.Name)
                .IsRequired();

            builder.Property(d => d.Description);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.CreatedBy)
                .IsRequired();

            builder.Property(d => d.UpdatedAt);

            builder.Property(d => d.LastUpdatedBy)
                .IsRequired();

            builder.HasIndex(d => new { d.OrganizationId, d.Name })
                .IsUnique();

            builder.HasOne(d => d.Organization)
                .WithMany(o => o.Departments)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Teams)
                .WithOne(t => t.Department)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
