using Microsoft.AspNetCore.Http;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using System;
using System.IO;
using System.Linq;

namespace Municiple_Project_st10259527.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public int GetTotalReports()
        {
            return _context.Reports.Count();
        }

        public List<ReportModel> GetReportsByUserId(int userId)
        {
            return _context.Reports
                           .Where(r => r.UserId == userId)
                           .OrderByDescending(r => r.ReportDate)
                           .ToList();
        }

        public List<ReportModel> GetRecentReports(int count = 5)
        {
            return _context.Reports
                           .OrderByDescending(r => r.ReportDate)
                           .Take(count)
                           .ToList();
        }

        public void AddReport(ReportModel report, IFormFile? uploadedFile)
        {
            // Set defaults for required fields
            report.ReportDate = DateTime.Now;
            report.Status ??= "Pending";
            report.ReportType ??= "General";
            report.Description ??= report.Description ?? "";
            report.Location ??= report.Location ?? "";

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadedFile.CopyTo(stream);
                }

                report.FilePath = "/uploads/" + uniqueFileName; // relative path
            }

            _context.Reports.Add(report);
            _context.SaveChanges();
        }

    }
}
