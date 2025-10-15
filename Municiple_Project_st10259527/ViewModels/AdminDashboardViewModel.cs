using System.Collections.Generic;
using Municiple_Project_st10259527.Models;
using System.Linq;

namespace Municiple_Project_st10259527.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Report Collections
        public IEnumerable<ReportModel> PendingReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> InProgressReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> CompletedReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> ApprovedReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> RejectedReports { get; set; } = Enumerable.Empty<ReportModel>();
        public IEnumerable<ReportModel> RecentReports { get; set; } = Enumerable.Empty<ReportModel>();

        // Counts
        public int TotalReports { get; set; }
        public int TotalUsers { get; set; }

        // Computed Properties
        public int PendingCount => PendingReports?.Count() ?? 0;
        public int InProgressCount => InProgressReports?.Count() ?? 0;
        public int CompletedCount => CompletedReports?.Count() ?? 0;
        public int ApprovedCount => ApprovedReports?.Count() ?? 0;
        public int RejectedCount => RejectedReports?.Count() ?? 0;

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
