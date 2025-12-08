using Microsoft.EntityFrameworkCore;
using Kursai.Api.Models;

namespace Kursai.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasMany(e => e.Courses)
                    .WithOne(e => e.Seller)
                    .HasForeignKey(e => e.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Course configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.CreatedAt);
            });

            // Favorite configuration
            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Favorites)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.Favorites)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Purchase configuration
            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Purchases)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                    .WithMany(e => e.Purchases)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed demo user with hashed password
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "demo",
                    Email = "demo@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123"),
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed demo courses
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "C# Complete Course",
                    Description = "Learn C# from basics to advanced",
                    Price = 49.99m,
                    SellerId = 1,
                    Category = "Programming",
                    ImageUrl = "dotnet_bot.png",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Course
                {
                    Id = 2,
                    Title = "Java Fundamentals",
                    Description = "Master Java programming",
                    Price = 39.99m,
                    SellerId = 1,
                    Category = "Programming",
                    ImageUrl = "dotnet_bot.png",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Course
                {
                    Id = 3,
                    Title = "Python for Beginners",
                    Description = "Start your Python journey",
                    Price = 29.99m,
                    SellerId = 1,
                    Category = "Programming",
                    ImageUrl = "dotnet_bot.png",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            );
        }
    }
}
