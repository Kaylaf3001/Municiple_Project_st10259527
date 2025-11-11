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

            // Seed data has been moved to SeedData.cs
        }
        //===================================================================================
    }
}
//====================================End=of=File=============================================

