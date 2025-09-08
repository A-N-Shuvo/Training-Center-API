namespace TrainingCenter_Api.Models.DTOs
{
    public class AssessmentDetailDTO
    {
        public int TraineeId { get; set; }
        public string AssessmentType { get; set; }
        public int TheoreticalScore { get; set; }
        public int PracticalScore { get; set; }
        public int DaysPresent { get; set; }
        public int TotalDays { get; set; }
        public string ParticipationLevel { get; set; }
        public string TechnicalSkillsRating { get; set; }
        public string CommunicationSkillsRating { get; set; }
        public string TeamworkRating { get; set; }
        public string DisciplineRemarks { get; set; }
        public string Punctuality { get; set; }
        public string AttitudeRating { get; set; }
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string ImprovementAreas { get; set; }
        public string TrainerRemarks { get; set; }
        public bool IsFinalized { get; set; }
    }

    public class AssessmentCreateDTO
    {
        public DateOnly AssessmentDate { get; set; }
        public int BatchId { get; set; }
        public int InstructorId { get; set; }
        public List<AssessmentDetailDTO> Assessments { get; set; }
    }

    public class AssessmentDTO
    {
        public int AssessmentId { get; set; }
        public DateOnly AssessmentDate { get; set; }

        public string? BatchName { get; set; }
        public string? InstructorName { get; set; }
        public string? TraineeName { get; set; }

        public string AssessmentType { get; set; }
        public int TheoreticalScore { get; set; }
        public int PracticalScore { get; set; }
        public decimal? OverallScore { get; set; }
    }

    public class AssessmentDetailssDTO
    {
        public int AssessmentId { get; set; }
        public DateOnly AssessmentDate { get; set; }

        public int BatchId { get; set; }
        public string? BatchName { get; set; }

        public int InstructorId { get; set; }
        public string? InstructorName { get; set; }

        public int TraineeId { get; set; }
        public string? TraineeName { get; set; }
        public string? traineeIDNo { get; set; }
        

        public string AssessmentType { get; set; }

        public int TheoreticalScore { get; set; }
        public int PracticalScore { get; set; }
        public decimal? OverallScore { get; set; }

        public int DaysPresent { get; set; }
        public int TotalDays { get; set; }
        public decimal AttendancePercentage { get; set; }
        public string ParticipationLevel { get; set; }

        public string TechnicalSkillsRating { get; set; }
        public string CommunicationSkillsRating { get; set; }
        public string TeamworkRating { get; set; }

        public string DisciplineRemarks { get; set; }
        public string Punctuality { get; set; }
        public string AttitudeRating { get; set; }

        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string ImprovementAreas { get; set; }
        public string TrainerRemarks { get; set; }

        public bool IsFinalized { get; set; }
    }



}

