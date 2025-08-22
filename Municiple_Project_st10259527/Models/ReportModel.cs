using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Municiple_Project_st10259527.Models
{
    //=================================================================
    // Report Model
    //=================================================================
    public class ReportModel
    {
        [Key]
        public int ReportId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string ReportType { get; set; }
        public string Description { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }

    }
    //=================================================================
}
