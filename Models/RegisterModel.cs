using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class RegisterModel
    {
        public string? UserId { get; set; }
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string ContactNo { get; set; }   
        
        [Required]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

    }

    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

