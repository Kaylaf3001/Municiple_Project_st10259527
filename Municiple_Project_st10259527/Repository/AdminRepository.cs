using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Repository;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repository
{

    public class AdminRepository : IAdminRepository
    {
        //===================================================================================
        // Admin Repository Implementation
        //===================================================================================
        #region
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        #endregion
        //===================================================================================

        //===================================================================================
        // Dashboard Methods
        //===================================================================================
        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var allReports = await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var oneWeekAgo = now.AddDays(-7);
            var today = now.Date;

            // Get reports by status for the dashboard tabs
            var pendingReports = allReports
                .Where(r => r.Status == ReportStatus.Pending)
                .ToList();

            var inProgressReports = allReports
                .Where(r => r.Status == ReportStatus.InReview)
                .ToList();

            var completedReports = allReports
                .Where(r => r.Status == ReportStatus.Completed)
                .ToList();
                
            var approvedReports = allReports
                .Where(r => r.Status == ReportStatus.Approved)
                .ToList();
                
            var rejectedReports = allReports
                .Where(r => r.Status == ReportStatus.Rejected)
                .ToList();

            // Get recent reports (all statuses, limited to 10)
            var recentReports = allReports.Take(10).ToList();

            // Get statistics
            var reportCounts = allReports
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var newReportsToday = await GetNewReportsCountTodayAsync();
            var resolvedThisWeek = await GetResolvedReportsCountThisWeekAsync();
            var averageResolutionHours = await GetAverageResolutionTimeHoursAsync();

            // Get top reporter
            var topReporter = allReports
                .GroupBy(r => r.UserId)
                .Select(g => new
                {
                    User = g.First().User,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            // Get additional data
            var topReporterInfo = await GetTopReporterInfoAsync();
            var statusDistribution = await GetStatusDistributionAsync();

            // Get top reporter info
            var (topReporterName, reportCount) = topReporterInfo;

            // Create and return the view model
            return new AdminDashboardViewModel
            {
                // Report collections
                PendingReports = pendingReports,
                InProgressReports = inProgressReports,
                CompletedReports = completedReports,
                ApprovedReports = approvedReports,
                RejectedReports = rejectedReports,
                RecentReports = recentReports,
                TotalReports = allReports.Count,
                TotalUsers = await _context.Users.CountAsync(),
                NewReportsToday = newReportsToday,
                ResolvedThisWeek = resolvedThisWeek,
                AverageResolutionTimeHours = averageResolutionHours,
                StatusDistribution = statusDistribution,
                TopReporter = topReporter?.User?.FullName ?? "N/A",
                ReportsByTopReporter = topReporter?.Count ?? 0
            };
        }
        //===================================================================================

        //===================================================================================
        // Report Management Methods
        //===================================================================================
        public async Task<ReportModel> GetReportByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }

        public async Task UpdateReportStatusAsync(int reportId, ReportStatus status, string adminNotes = null)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report != null)
            {
                report.Status = status;
                report.AdminNotes = adminNotes;
                report.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        //===================================================================================

        //===================================================================================
        // Additional Methods
        //===================================================================================
        public async Task<IEnumerable<ReportModel>> GetReportsAsync(ReportStatus? status = null, int page = 1, int pageSize = 10, string searchTerm = null)
        {
            IQueryable<ReportModel> query = _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.ReportDate);

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(r =>
                    r.Description.ToLower().Contains(searchTerm) ||
                    r.ReportType.ToLower().Contains(searchTerm) ||
                    r.Location.ToLower().Contains(searchTerm) ||
                    (r.User.FirstName + " " + r.User.LastName).ToLower().Contains(searchTerm) ||
                    r.User.Email.ToLower().Contains(searchTerm));
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        //===================================================================================

        //===================================================================================
        // Helper Methods
        //===================================================================================
        public async Task<int> GetReportCountByStatusAsync(ReportStatus status)
        {
            return await _context.Reports.CountAsync(r => r.Status == status);
        }
        //===================================================================================

        //===================================================================================
        // Statistics and Analytics Methods
        //===================================================================================
        public async Task<int> GetTotalReportCountAsync()
        {
            return await _context.Reports.CountAsync();
        }
        //===================================================================================

        //===================================================================================
        // Additional Helper Methods
        //===================================================================================
        public async Task<IEnumerable<ReportModel>> GetReportsByStatusAsync(ReportStatus status)
        {
            return await _context.Reports
                .Include(r => r.User)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }
        //===================================================================================

        //===================================================================================
        // More Helper Methods
        //===================================================================================
        public async Task<IEnumerable<ReportModel>> GetAllReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();
        }
        //===================================================================================

        //===================================================================================
        // Even More Helper Methods
        //===================================================================================
        public async Task<Dictionary<string, int>> GetStatusDistributionAsync()
        {
            var allReports = await _context.Reports.ToListAsync();

            return allReports
                .GroupBy(r => r.Status)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Count()
                );
        }
        //===================================================================================

        //===================================================================================
        // Final Helper Methods
        //===================================================================================
        public async Task<(string topReporter, int reportCount)> GetTopReporterInfoAsync()
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .Where(r => r.User != null)
                .ToListAsync();

            var topReporter = reports
                .GroupBy(r => r.UserId)
                .Select(g => new
                {
                    User = g.First().User,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();

            return (topReporter?.User?.FullName ?? "N/A", topReporter?.Count ?? 0);
        }
        //===================================================================================

        //===================================================================================
        // Private Methods for Statistics
        //===================================================================================
        public async Task<int> GetNewReportsCountTodayAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Reports
                .CountAsync(r => r.ReportDate >= today);
        }
        //===================================================================================

        //===================================================================================
        // Private Methods for Statistics
        //===================================================================================
        public async Task<int> GetResolvedReportsCountThisWeekAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            return await _context.Reports
                .CountAsync(r =>
                    (r.Status == ReportStatus.Completed || r.Status == ReportStatus.Rejected) &&
                    r.UpdatedAt >= oneWeekAgo);
        }
        //===================================================================================

        //===================================================================================
        // Private Methods for Statistics
        //===================================================================================
        public async Task<double> GetAverageResolutionTimeHoursAsync()
        {
            var resolvedReports = await _context.Reports
                .Where(r => r.Status == ReportStatus.Completed && r.UpdatedAt.HasValue)
                .ToListAsync();

            if (!resolvedReports.Any())
                return 0;

            var totalHours = resolvedReports
                .Where(r => r.UpdatedAt.HasValue)
                .Average(r => (r.UpdatedAt.Value - r.ReportDate).TotalHours);

            return Math.Round(totalHours, 2);
        }
        //===================================================================================

        //===================================================================================
        // Additional Methods
        //===================================================================================
        public async Task<int> GetTotalUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }
        //===================================================================================

        //===================================================================================
        // Get recent reports by status
        //===================================================================================
        public async Task<List<ReportModel>> GetRecentReportsByStatusAsync(ReportStatus status, int count)
        {
            return await _context.Reports
                .Include(r => r.User)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.ReportDate)
                .Take(count)
                .ToListAsync();
        }
        //===================================================================================
    }
}
//=============================End=Of=File====================================================