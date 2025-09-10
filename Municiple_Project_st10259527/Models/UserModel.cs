using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Municiple_Project_st10259527.Models
{
    //=================================================================
    // User Model
    //=================================================================
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Admin User")]
        public bool IsAdmin { get; set; } = false;

        // Navigation property for reports created by this user
        public virtual ICollection<ReportModel> Reports { get; set; }

        // Navigation property for reports assigned to this admin
        public virtual ICollection<ReportModel> AssignedReports { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
    //=================================================================
}
