namespace TrainingCenter_Api.Models.DTOs
{
    public class RecommendationDetailDTO
    {
        public int TraineeId { get; set; }
        public int AssessmentId { get; set; }
        public int InvoiceId { get; set; }
        public string RecommendationText { get; set; }
        public string RecommendationStatus { get; set; } // pending,Approved,Rejected
    }

    public class RecommendationCreateDTO
    {
        public DateOnly RecommendationDate { get; set; }
        public int BatchId { get; set; }
        public int InstructorId { get; set; }
        public List<RecommendationDetailDTO>? Recommendations { get; set; }
    }
    public class RecommendationDTO
    {
        public int RecommendationId { get; set; }
        public DateOnly RecommendationDate { get; set; }
        public int InstructorId { get; set; }
        public int TraineeId { get; set; }
        public int BatchId { get; set; }
        public int AssessmentId { get; set; }  
        public int InvoiceId { get; set; }

        public string? BatchName { get; set; }
        public string? InstructorName { get; set; }
        public string? TraineeName { get; set; }
        public string RecommendationStatus { get; set; }
        public string RecommendationText { get; set; }

    }

    public class RecommendationDetailssDTO
    {
        public int RecommendationId { get; set; }
        public DateOnly RecommendationDate { get; set; }

        public int BatchId { get; set; }
        public string? BatchName { get; set; }

        public int InstructorId { get; set; }
        public string? InstructorName { get; set; }

        public int TraineeId { get; set; }
        public string? TraineeName { get; set; }

        public int AssessmentId { get; set; }
        public bool? IsFinalized { get; set; }

        public int InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }

        public string RecommendationText { get; set; }

        public string RecommendationStatus { get; set; } // pending,Approved,Rejected
    }
}
