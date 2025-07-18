using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }

        [Required]
        [Display(Name = "Trainee name")]
        public int TraineeId { get; set; }
        public Trainee? Trainee { get; set; }

        [Required]
        [Display(Name = "Course name")]

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Certificate number")]

        public string CertificateNumber { get; set; } // ইউনিক সার্টিফিকেট নম্বর

        // Registration এর রেফারেন্স (যদি প্রয়োজন হয়)
        [Display(Name = "Registration number")]

        public int RegistrationId { get; set; }
        public Registration? Registration { get; set; }

        // Invoice রেফারেন্স (যেখানে Due 0)
        [Required]
        [Display(Name = "Invoice")]

        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        // Instructor এর Recommendation রেফারেন্স (Certificate ইস্যুর পূর্বশর্ত)
        [Required]
        [Display(Name = "Recommendation")]

        public int RecommendationId { get; set; }
        public Recommendation? Recommendation { get; set; }
    }
}
