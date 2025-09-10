using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Data;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var allReports = await _context.Reports
                .Include(r => r.User)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalReports = allReports.Count,
                TotalUsers = await _context.Users.CountAsync(),
                PendingReports = allReports
                    .Where(r => r.Status == ReportStatus.Pending)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(5)
                    .ToList(),
                InProgressReports = allReports
                    .Where(r => r.Status == ReportStatus.InReview)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(5)
                    .ToList(),
                CompletedReports = allReports
                    .Where(r => r.Status == ReportStatus.Completed)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(5)
                    .ToList(),
                ApprovedReports = allReports
                    .Where(r => r.Status == ReportStatus.Approved)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(5)
                    .ToList(),
                RecentReports = await _context.Reports
                    .Include(r => r.User)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(10)
                    .ToListAsync(),
                // Initialize statistics
                NewReportsToday = allReports.Count(r => r.ReportDate.Date == DateTime.Today),
                ResolvedThisWeek = allReports.Count(r => 
                    (r.Status == ReportStatus.Completed || r.Status == ReportStatus.Approved) && 
                    r.UpdatedAt.HasValue && 
                    r.UpdatedAt.Value >= DateTime.Today.AddDays(-7))
            };

            // Calculate status distribution
            viewModel.StatusDistribution = allReports
                .GroupBy(r => r.Status)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Count()
                );

            // Calculate top reporter
            var topReporter = allReports
                .GroupBy(r => r.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            if (topReporter != null)
            {
                var user = await _context.Users.FindAsync(topReporter.UserId);
                if (user != null)
                {
                    viewModel.TopReporter = $"{user.FirstName} {user.LastName}";
                    viewModel.ReportsByTopReporter = topReporter.Count;
                }
            }

            return viewModel;
        }

        public async Task<ReportModel> GetReportByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task UpdateReportStatusAsync(int reportId, ReportStatus status, string adminNotes = null)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
            {
                throw new Exception($"Report with ID {reportId} not found");
            }
            if (report != null)
            {
                report.Status = status;
                report.AdminNotes = adminNotes;
                report.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReportModel>> GetReportsAsync(ReportStatus? status = null, int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var query = _context.Reports
                .Include(r => r.User)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r => 
                    r.Title.Contains(searchTerm) || 
                    r.Description.Contains(searchTerm) ||
                    r.User.Email.Contains(searchTerm));
            }

            return await query
                .OrderByDescending(r => r.ReportDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetReportCountByStatusAsync(ReportStatus status)
        {
            return await _context.Reports
                .CountAsync(r => r.Status == status);
        }

        public async Task<int> GetTotalReportCountAsync()
        {
            return await _context.Reports.CountAsync();
        }

        public async Task<List<ReportModel>> GetRecentReportsAsync(int count)
        {
            return await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.ReportDate)
                .Take(count)
                .ToListAsync();
        }

        private async Task<List<ReportModel>> GetRecentReportsByStatusAsync(ReportStatus status, int count)
        {
            return await _context.Reports
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.ReportDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
