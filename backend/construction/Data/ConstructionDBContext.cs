using ConstructionProjectManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace ConstructionProjectManagement.Data
{ 
    public class ConstructionDbContext : DbContext
    {
        public ConstructionDbContext(DbContextOptions<ConstructionDbContext> options) : base(options) { }

        public DbSet<ConstructionProject> ConstructionProjects { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ConstructionProject>()
            //    .Property(p => p.ProjectId)
            //    .HasDefaultValueSql("gen_random_uuid()");

            //modelBuilder.Entity<User>()
            //    .Property(u => u.UserId)
            //    .HasDefaultValueSql("gen_random_uuid()");
            modelBuilder.Entity<ConstructionProject>()
              .Property(p => p.ProjectStage)
              .HasConversion(new EnumToNumberConverter<ProjectStage, int>());

            modelBuilder.Entity<ConstructionProject>()
              .Property(p => p.ProjectCategory) 
              .HasConversion(new EnumToNumberConverter<ProjectCategory, int>());


            modelBuilder.Entity<ConstructionProject>()
     .HasIndex(p => p.ProjectName)
                .IsUnique();


            //Add initial users
            modelBuilder.Entity<User>().HasData(
                new User { UserId = Guid.Parse("12345678-1234-1234-1234-1234567890ab"), Email = "test@admin.com", Password = BCrypt.Net.BCrypt.HashPassword("admin123") }
            );
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new DateTimeValueConverter());
                    }
                }
            }
        }

        public class DateTimeValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
        {
            public DateTimeValueConverter()
                : base(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            {
            }
        }
    }
}