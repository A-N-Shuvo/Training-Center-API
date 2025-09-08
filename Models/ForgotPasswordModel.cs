using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
