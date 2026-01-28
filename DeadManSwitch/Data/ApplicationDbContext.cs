using DeadManSwitch.Models;
using DeadManSwitch.Services;
using Microsoft.EntityFrameworkCore;

namespace DeadManSwitch.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public void Seed(IPasswordService passwordService)
        {
            if (!Users.Any())
            {
                passwordService.CreatePasswordHash("admin123", out byte[] passwordHash, out byte[] passwordSalt);
                Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                });
                SaveChanges();
            }
        }
    }
}
