using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municiple_Project_st10259527.Models
{
    public class AnnouncementModel
    {
        [Key]
        public int AnnouncementId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string Location { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Announced, Cancelled, etc.

        // Foreign Key for Admin
        [ForeignKey("Admin")]
        public int UserId { get; set; }

        // Navigation property for Admin
        public UserModel? User{ get; set; }

    }
}
