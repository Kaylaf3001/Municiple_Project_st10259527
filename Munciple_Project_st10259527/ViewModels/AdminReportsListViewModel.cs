using System.Collections.Generic;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class AdminReportsListViewModel
    {
        public IEnumerable<ReportModel> Reports { get; set; }
        public ReportStatus? FilterStatus { get; set; }
        
        // For pagination
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        
        // For search/filter
        public string SearchTerm { get; set; }
        
        // Counts for status filters
        public int PendingCount { get; set; }
        public int InReviewCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int CompletedCount { get; set; }
        
        // For the status filter dropdown
        public Dictionary<ReportStatus, string> StatusOptions => new Dictionary<ReportStatus, string>
        {
            { ReportStatus.Pending, "Pending" },
            { ReportStatus.InReview, "In Review" },
            { ReportStatus.Approved, "Approved" },
            { ReportStatus.Rejected, "Rejected" },
            { ReportStatus.Completed, "Completed" }
        };
    }
}
