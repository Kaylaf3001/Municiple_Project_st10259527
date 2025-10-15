using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Municiple_Project_st10259527.Models
{
    //=================================================================
    // User Search History Model
    //=================================================================
    public class UserSearchHistory
    {
        [Key]
        public int SearchId { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(100)]
        public string? SearchTerm { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [Required]
        public DateTime SearchDate { get; set; } = DateTime.UtcNow;
    }
    //=================================================================
}

