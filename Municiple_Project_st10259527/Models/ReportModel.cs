using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municiple_Project_st10259527.Models
{
    public enum ReportStatus
    {
        Pending,
        InReview,
        Approved,
        Rejected,
        Completed
    }

    //=================================================================
    // Report Model
    //=================================================================
    public class ReportModel
    {
        [Key]
        public int ReportId { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }
        
        [Required]
        [Display(Name = "Report Type")]
        public string ReportType { get; set; }
        
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        
        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; } = DateTime.Now;
        
        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }
        
        [Display(Name = "Status")]
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        
        [Display(Name = "File")]
        public string? FilePath { get; set; }
        
        [Display(Name = "Admin Notes")]
        public string? AdminNotes { get; set; }
        
        [Display(Name = "Last Updated")]
        public DateTime? LastUpdated { get; set; }
        
        [Display(Name = "Assigned Admin")]
        public int? AssignedAdminId { get; set; }
        
        [ForeignKey("AssignedAdminId")]
        public virtual UserModel? AssignedAdmin { get; set; }
    }
}
