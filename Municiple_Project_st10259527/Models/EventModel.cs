using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Municiple_Project_st10259527.Models
{
    public class EventModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }  // e.g., Community, Safety, Infrastructure, Culture

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Upcoming, Cancelled, etc.

        // Foreign Key for Admin
        [ForeignKey("Admin")]
        public string AdminId { get; set; }  // assuming you use string for Identity user ID
    }
}
