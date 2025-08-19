using Microsoft.AspNetCore.Mvc;

namespace Municiple_Project_st10259527.Models
{
    //=================================================================
    // Report Model
    //=================================================================
    public class ReportModel
    {
        public string ReportId { get; set; }
        public string UserId { get; set; }
        public string ReportType { get; set; }
        public string Description { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }

    }
    //=================================================================
}
