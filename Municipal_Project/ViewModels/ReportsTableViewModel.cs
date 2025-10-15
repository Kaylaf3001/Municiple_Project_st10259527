using System.Collections.Generic;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class ReportsTableViewModel
    {
        public IEnumerable<ReportModel> Reports { get; set; }
        public string EmptyMessage { get; set; } = "No reports found.";
    }
}
