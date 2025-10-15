using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municiple_Project_st10259527.Models
{
    //=================================================================
    // Announcement Model
    //=================================================================
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
        public string Status { get; set; }

        [ForeignKey("Admin")]
        public int UserId { get; set; }

        public UserModel? User{ get; set; }

    }
    //=================================================================
}
