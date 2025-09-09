using System;
using System.ComponentModel.DataAnnotations;
using Municiple_Project_st10259527.Models;

namespace Municiple_Project_st10259527.ViewModels
{
    public class ReportReviewViewModel
    {
        public int ReportId { get; set; }

        [Display(Name = "Report Type")]
        public string ReportType { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Current Status")]
        public ReportStatus Status { get; set; }

        [Display(Name = "File")]
        public string FilePath { get; set; }

        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }

        [Display(Name = "User Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please select a status")]
        [Display(Name = "New Status")]
        public ReportStatus NewStatus { get; set; }

        [Display(Name = "Admin Notes")]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string AdminNotes { get; set; }
    }
}
