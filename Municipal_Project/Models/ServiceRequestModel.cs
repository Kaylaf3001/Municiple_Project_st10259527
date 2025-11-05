using System;
using System.ComponentModel.DataAnnotations;

namespace Municiple_Project_st10259527.Models
{
    public enum ServiceRequestStatus
    {
        Submitted,
        InProgress,
        OnHold,
        Completed,
        Cancelled
    }

    public class ServiceRequestModel
    {
        [Key]
        public int RequestId { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Submitted;
        
        public int Priority { get; set; } = 0; // lower is more urgent
        
        public string TrackingCode { get; set; } = Guid.NewGuid().ToString("N").ToUpperInvariant();
    }
}
