using System.Collections.Generic;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class UserReportsViewModel
    {
        public IEnumerable<ReportModel> Reports { get; set; }
        public int TotalReports { get; set; }
        public int ResolvedCount { get; set; }
        public int InReviewCount { get; set; }
        public int DeniedCount { get; set; }
        public int PendingCount { get; set; }
    }
}
