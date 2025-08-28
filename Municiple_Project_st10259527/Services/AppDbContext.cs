using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services
{
    //Note that we can not use lists we must use something else to stores the users and the reports
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ReportModel> Reports{ get; set; }
    }
}
