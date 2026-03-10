using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.DepartmentId)
                .IsRequired();

            builder.Property(t => t.Name)
                .IsRequired();

            builder.Property(t => t.Description);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'")
                .IsRequired();

            builder.Property(t => t.CreatedBy)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder.Property(t => t.LastUpdatedBy)
                .IsRequired();

            builder.HasIndex(t => new { t.DepartmentId, t.Name })
                .IsUnique();

            builder.HasOne(t => t.Department)
                .WithMany(d => d.Teams)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
