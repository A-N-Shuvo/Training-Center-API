using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
 
    public class Recommendation
    {
        [Key]
        public int RecommendationId { get; set; }
        public DateOnly RecommendationDate { get; set; }

        public int BatchId { get; set; }
        public Batch? Batch { get; set; }

        public int InstructorId { get; set; }
        public Instructor? Instructor { get; set; }

       //=================unique or individual================
        public int TraineeId { get; set; }
        public Trainee? Trainee { get; set; }

        public int AssessmentId { get; set; } 
        public Assessment? Assessment { get; set; }

        public int InvoiceId { get; set; } 
        public Invoice? Invoice { get; set; }

        public string RecommendationText { get; set; }       

        public string RecommendationStatus { get; set; } // pending,Approved,Rejected
    }
}
