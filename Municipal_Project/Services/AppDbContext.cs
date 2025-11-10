using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;
using System;

namespace Municiple_Project_st10259527.Services
{
    public class AppDbContext : DbContext
    {
        //===================================================================================
        // Constructor
        //===================================================================================
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ReportModel> Reports { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<AnnouncementModel> Announcements { get; set; }
        public DbSet<UserSearchHistory> UserSearchHistory { get; set; }
        public DbSet<ServiceRequestModel> ServiceRequests { get; set; }
        //===================================================================================

        //===================================================================================
        // Model Creating - Relationships and Seed Data
        //===================================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSearchHistory>()
                .HasKey(us => us.SearchId);

            modelBuilder.Entity<ReportModel>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReportModel>()
                .HasOne(r => r.AssignedAdmin)
                .WithMany(a => a.AssignedReports)
                .HasForeignKey(r => r.AssignedAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventModel>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AnnouncementModel>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceRequests configuration
            modelBuilder.Entity<ServiceRequestModel>()
                .Property(s => s.Status)
                .HasConversion<int>();

            modelBuilder.Entity<ServiceRequestModel>()
                .HasIndex(s => s.TrackingCode)
                .IsUnique();

            modelBuilder.Entity<UserModel>().HasData(
                new UserModel
                {
                    UserId = 1,
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@example.com",
                    Password = "Admin@123",
                    IsAdmin = true
                }
            );

            modelBuilder.Entity<EventModel>().HasData(
                new EventModel
                {
                    EventId = 1,
                    Title = "Community Cleanup Day",
                    Location = "Main Street Park",
                    Date = new DateTime(2025, 11, 20, 9, 0, 0),
                    Category = "Community",
                    Description = "Join neighbors to clean up the park and surrounding streets.",
                    Status = "Scheduled",
                    UserId = 1
                },
                new EventModel
                {
                    EventId = 2,
                    Title = "Town Hall Meeting",
                    Location = "Municipal Hall, Council Chamber",
                    Date = new DateTime(2025, 12, 1, 18, 30, 0),
                    Category = "Government",
                    Description = "Monthly town hall to discuss budget and development plans.",
                    Status = "Scheduled",
                    UserId = 1
                },
                new EventModel
                {
                    EventId = 3,
                    Title = "Holiday Market",
                    Location = "Riverside Square",
                    Date = new DateTime(2025, 12, 10, 16, 0, 0),
                    Category = "Culture",
                    Description = "Local vendors, crafts, and live music for the holidays.",
                    Status = "Scheduled",
                    UserId = 1
                },
                new EventModel
                {
                    EventId = 4,
                    Title = "Youth Sports Tryouts",
                    Location = "Community Sports Center",
                    Date = new DateTime(2026, 1, 15, 15, 0, 0),
                    Category = "Sports",
                    Description = "Open tryouts for municipal youth soccer and basketball.",
                    Status = "Scheduled",
                    UserId = 1
                },
                new EventModel
                {
                    EventId = 5,
                    Title = "Public Safety Workshop",
                    Location = "Fire Station #2",
                    Date = new DateTime(2026, 2, 5, 10, 0, 0),
                    Category = "Safety",
                    Description = "Learn fire safety, emergency kits, and first aid basics.",
                    Status = "Scheduled",
                    UserId = 1
                }
            );

            modelBuilder.Entity<AnnouncementModel>().HasData(
                new AnnouncementModel
                {
                    AnnouncementId = 1,
                    Title = "Water Service Interruption",
                    Date = new DateTime(2025, 11, 22, 8, 0, 0),
                    Description = "Planned maintenance will interrupt water service in the Eastside from 8am-2pm.",
                    Location = "Eastside District",
                    Status = "Published",
                    UserId = 1
                },
                new AnnouncementModel
                {
                    AnnouncementId = 2,
                    Title = "Recycling Schedule Update",
                    Date = new DateTime(2025, 11, 25, 0, 0, 0),
                    Description = "Recycling pickup moves to Fridays for Zone B starting next week.",
                    Location = "Zone B",
                    Status = "Published",
                    UserId = 1
                },
                new AnnouncementModel
                {
                    AnnouncementId = 3,
                    Title = "Road Closure Notice",
                    Date = new DateTime(2025, 12, 2, 6, 0, 0),
                    Description = "Elm Street will be closed for resurfacing. Use Oak Ave detour.",
                    Location = "Elm Street",
                    Status = "Published",
                    UserId = 1
                },
                new AnnouncementModel
                {
                    AnnouncementId = 4,
                    Title = "Heat Advisory",
                    Date = new DateTime(2025, 12, 15, 12, 0, 0),
                    Description = "High temperatures expected. Cooling centers open 10am-7pm.",
                    Location = "Citywide",
                    Status = "Published",
                    UserId = 1
                },
                new AnnouncementModel
                {
                    AnnouncementId = 5,
                    Title = "Library Renovation",
                    Date = new DateTime(2026, 1, 5, 9, 0, 0),
                    Description = "Main library will undergo renovations; temporary location on Pine St.",
                    Location = "Main Library",
                    Status = "Published",
                    UserId = 1
                }
            );
        }
        //===================================================================================
    }
}
//====================================End=of=File=============================================

