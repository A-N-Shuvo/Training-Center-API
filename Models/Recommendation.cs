using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public enum RecommendationCategory
    {
        AcademicPerformance,
        Attendance,
        Behavior,
        SkillCompetency,
        OverallEvaluation,
        Other
    }

    public enum RecommendationStatus
    {
        Approved,
        Pending,
        Rejected
    }
    public class Recommendation
    {
        [Key]
        public int RecommendationId { get; set; }
        [Required]
        [Display(Name = "Batch name")]

        public int BatchId { get; set; }
        public Batch? Batch { get; set; }

        [Required]
        [Display(Name = "Instructor name")]

        public int InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

        [Required]

        public int TraineeId { get; set; }
        public Trainee? Trainee { get; set; }

        //[Required]
        //[Display(Name = "Admission")]

        //public int AdmissionId { get; set; }
        //public Admission? Admission { get; set; }

        [Required]
        public RecommendationCategory Category { get; set; }

        [Required]
        [MaxLength(2000)]
        public string RecommendationText { get; set; }

        [Required]
        [DataType(DataType.Date)]

        public DateTime RecommendationDate { get; set; }

        [MaxLength(50)]
        public RecommendationStatus? Status { get; set; } // যেমন: Approved, Pending, Rejected

        [Required]
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
