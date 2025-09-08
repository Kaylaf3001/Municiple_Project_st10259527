using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ReportModel> Reports { get; set; }

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
        }
    }
}
