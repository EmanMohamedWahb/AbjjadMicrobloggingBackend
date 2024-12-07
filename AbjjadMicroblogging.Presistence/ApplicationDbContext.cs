using AbjjadMicroblogging.Domain;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Generic;

namespace AbjjadMicroblogging.Presistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; } 
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed a test user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "user1",
                    Password = BCrypt.Net.BCrypt.HashPassword("password1"),
                    Email = "test@test.com",
                    Name = "user1"
                }
            );
        }
    }
}
