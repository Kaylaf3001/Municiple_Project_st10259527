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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime ReportDate { get; set; }
        
        [Display(Name = "Location")]
        public string Location { get; set; }
        
        [Display(Name = "Status")]
        public ReportStatus Status { get; set; }
        
        [Display(Name = "File")]
        public string FilePath { get; set; }
        
        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }
        
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
        
        // For the status update form
        [Required]
        [Display(Name = "Update Status")]
        public ReportStatus NewStatus { get; set; }
        
        [Display(Name = "Admin Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        public string AdminNotes { get; set; }
        
        // For displaying the file if it's an image
        public bool IsImageFile
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return false;
                    
                var extension = System.IO.Path.GetExtension(FilePath)?.ToLower();
                return extension == ".jpg" || extension == ".jpeg" || 
                       extension == ".png" || extension == ".gif";
            }
        }
        
        // For displaying the file name
        public string FileName => System.IO.Path.GetFileName(FilePath);
    }
}
