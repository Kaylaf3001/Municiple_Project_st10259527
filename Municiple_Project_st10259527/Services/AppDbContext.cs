using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

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
        //===================================================================================

        //===================================================================================
        // Model Creating - Relationships and Seed Data
        //===================================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Report and User (who created the report)
            modelBuilder.Entity<ReportModel>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between Report and AssignedAdmin
            modelBuilder.Entity<ReportModel>()
                .HasOne(r => r.AssignedAdmin)
                .WithMany(a => a.AssignedReports)
                .HasForeignKey(r => r.AssignedAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Events ↔ Admin
            modelBuilder.Entity<EventModel>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Announcements ↔ Admin
            modelBuilder.Entity<AnnouncementModel>()
                .HasOne(a => a.Admin)
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Seed a default admin user
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
        }
        //===================================================================================
    }
}
//====================================End=of=File=============================================
