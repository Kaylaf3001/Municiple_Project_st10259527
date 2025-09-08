using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Municiple_Project_st10259527.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
