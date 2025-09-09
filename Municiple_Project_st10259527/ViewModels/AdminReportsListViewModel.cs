using Municiple_Project_st10259527.Models;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.ViewModels
{
    public class AdminReportsListViewModel
    {
        public IEnumerable<ReportModel> Reports { get; set; } = new List<ReportModel>();
        public ReportStatus? FilterStatus { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public int PendingCount { get; set; }
        public int InReviewCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int CompletedCount { get; set; }
    }
}