using System.Collections.Generic;
using Municiple_Project_st10259527.Models;
using System.Linq;

namespace Municiple_Project_st10259527.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Report Collections
        public IEnumerable<ReportModel> PendingReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> InReviewReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> RecentReports { get; set; } = Enumerable.Empty<ReportModel>();
        
        // Counts
        public int TotalReports { get; set; }
        public int TotalUsers { get; set; }
        public int PendingCount { get; set; }
        public int InReviewCount { get; set; }
        public int CompletedCount { get; set; }
        public int RejectedCount { get; set; }
        public int ApprovedCount { get; set; }
        
        // Computed Properties
        public int InProgressCount => InReviewCount;
        
        // Statistics
        public int NewReportsToday { get; set; }
        public int ResolvedThisWeek { get; set; }
        public double AverageResolutionTimeHours { get; set; }
        
        // User Statistics
        public string TopReporter { get; set; } = "N/A";
        public int ReportsByTopReporter { get; set; }
        
        // Status Distribution
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
    }
}
