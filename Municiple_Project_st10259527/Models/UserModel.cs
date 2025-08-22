using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Municiple_Project_st10259527.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
