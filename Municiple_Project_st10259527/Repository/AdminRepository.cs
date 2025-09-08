using Microsoft.EntityFrameworkCore;
using Municiple_Project_st10259527.Models;
using Municiple_Project_st10259527.Services;
using Municiple_Project_st10259527.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Municiple_Project_st10259527.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var allReports = await _context.Reports
                .Include(r => r.User)
                .ToListAsync();

            var now = DateTime.UtcNow;
            var oneWeekAgo = now.AddDays(-7);
            var today = now.Date;

            // Get reports by status
            var pendingReports = allReports
                .Where(r => r.Status == ReportStatus.Pending)
                .OrderByDescending(r => r.ReportDate)
                .Take(10);

            var inReviewReports = allReports
                .Where(r => r.Status == ReportStatus.InReview)
                .OrderByDescending(r => r.ReportDate)
                .Take(10);

            // Get statistics
            var reportCounts = allReports
                .GroupBy(r => r.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var newReportsToday = await GetNewReportsCountTodayAsync();
            var resolvedThisWeek = await GetResolvedReportsCountThisWeekAsync();
            var averageResolutionHours = await GetAverageResolutionTimeHoursAsync();
            var topReporterInfo = await GetTopReporterInfoAsync();
            var statusDistribution = await GetStatusDistributionAsync();

            var viewModel = new AdminDashboardViewModel
            {
                // Report collections
                PendingReports = pendingReports.ToList(),
                InProgressReports = allReports
                    .Where(r => r.Status == ReportStatus.InReview)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(10)
                    .ToList(),
                CompletedReports = allReports
                    .Where(r => r.Status == ReportStatus.Completed)
                    .OrderByDescending(r => r.ReportDate)
                    .Take(10)
                    .ToList(),
                RecentReports = allReports
                    .OrderByDescending(r => r.ReportDate)
                    .Take(10)
                    .ToList(),
                
                // Counts
                TotalReports = allReports.Count,
                TotalUsers = await _context.Users.CountAsync(),
                
                // Status Distribution
                StatusDistribution = allReports
                    .GroupBy(r => r.Status)
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => g.Count()
                    )
            };

            return viewModel;
        }

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

        public async Task<int> GetNewReportsCountTodayAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Reports
                .CountAsync(r => r.ReportDate >= today);
        }

        public async Task<int> GetResolvedReportsCountThisWeekAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
            return await _context.Reports
                .CountAsync(r => 
                    (r.Status == ReportStatus.Completed || r.Status == ReportStatus.Rejected) &&
                    r.LastUpdated >= oneWeekAgo);
        }

        public async Task<double> GetAverageResolutionTimeHoursAsync()
        {
            var resolvedReports = await _context.Reports
                .Where(r => r.Status == ReportStatus.Completed && r.LastUpdated.HasValue)
                .ToListAsync();

            if (!resolvedReports.Any())
                return 0;

            return resolvedReports
                .Average(r => (r.LastUpdated!.Value - r.ReportDate).TotalHours);
        }
    }
}
