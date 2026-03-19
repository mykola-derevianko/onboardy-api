using Microsoft.EntityFrameworkCore;
using OnBoardy.API.Models;

namespace OnBoardy.API.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public required DbSet<User> Users { get; set; }
        public required DbSet<Organization> Organizations { get; set; }
        public required DbSet<Membership> Memberships { get; set; }
        public required DbSet<RefreshToken> RefreshTokens { get; set; }
        public required DbSet<EmailVerification> EmailVerification { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            optionsBuilder
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }
    }
}
