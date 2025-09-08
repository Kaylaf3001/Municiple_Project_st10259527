using System.Collections.Generic;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Report Collections
        public List<ReportModel> PendingReports { get; set; } = new List<ReportModel>();
        public List<ReportModel> InProgressReports { get; set; } = new List<ReportModel>();
        public List<ReportModel> CompletedReports { get; set; } = new List<ReportModel>();
        public List<ReportModel> RecentReports { get; set; } = new List<ReportModel>();
        
        // Counts
        public int TotalReports { get; set; }
        public int TotalUsers { get; set; }
        
        // Computed Properties
        public int PendingCount => PendingReports.Count;
        public int InProgressCount => InProgressReports.Count;
        public int CompletedCount => CompletedReports.Count;
        
        // Status Distribution
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
    }
}
