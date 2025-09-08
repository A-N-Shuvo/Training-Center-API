using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }
        public string? CertificateNumber { get; set; } //Example-- CR-01

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }

        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }
        public int TraineeId { get; set; }
        public virtual Trainee? Trainee { get; set; }
        public int RegistrationId { get; set; }
        public virtual Registration? Registration { get; set; }
        public int RecommendationId { get; set; }
        public virtual Recommendation? Recommendation { get; set; }

    }
}
