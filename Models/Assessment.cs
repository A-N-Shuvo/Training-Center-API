namespace TrainingCenter_Api.Models
{
    public class Assessment
    {
        // Basic Assessment Information
        public int AssessmentId { get; set; }
        public int TraineeId { get; set; }
        public int BatchId { get; set; }
        public int InstructorId { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string AssessmentType { get; set; } // e.g., "Weekly Test", "Final Evaluation", "Behavioral"

        // Performance Metrics
        public decimal? TheoreticalScore { get; set; } // থিওরেটিকাল স্কোর (০-১০০)
        public decimal? PracticalScore { get; set; } // প্র্যাকটিক্যাল স্কোর (০-১০০)
        public decimal? OverallScore { get; set; } // সামগ্রিক স্কোর (স্বয়ংক্রিয়ভাবে ক্যালকুলেট করা যায়)

        // Attendance & Participation
        public int DaysPresent { get; set; }
        public int TotalDays { get; set; }
        public decimal AttendancePercentage => TotalDays > 0 ? (DaysPresent * 100m / TotalDays) : 0;
        public string ParticipationLevel { get; set; } // "Low", "Medium", "High"

        // Skill Evaluation (can use enum or constants)
        public string TechnicalSkillsRating { get; set; } // "Poor", "Average", "Good", "Excellent"
        public string CommunicationSkillsRating { get; set; }
        public string TeamworkRating { get; set; }

        // Behavioral Assessment
        public string DisciplineRemarks { get; set; }
        public string Punctuality { get; set; } // "Early", "On Time", "Late"
        public string AttitudeRating { get; set; } // 1-5 scale

        // Trainer's Qualitative Evaluation
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string ImprovementAreas { get; set; }
        public string TrainerRemarks { get; set; }

        // Additional Metadata
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsFinalized { get; set; } // ট্রেইনার ফাইনালাইজ করেছেন কিনা

        // Navigation properties (if using Entity Framework)
        public virtual Trainee? Trainee { get; set; }
        public virtual Batch? Batch { get; set; }
        public virtual Instructor? Instructor { get; set; }

        // Methods
        public void CalculateOverallScore()
        {
            // কাস্টম লজিক হিসেবে থিওরেটিকাল ও প্র্যাকটিক্যাল স্কোরের গড় নেয়া হলো
            if (TheoreticalScore.HasValue && PracticalScore.HasValue)
            {
                OverallScore = (TheoreticalScore.Value + PracticalScore.Value) / 2;
            }
        }
    }

    // Optional: Supporting classes/enums
    public enum TechnicalSkillsRating { Poor, Average, Good, Excellent }
    public enum ParticipationLevel { Low, Medium, High }
    public enum CommunicationSkillsRating { Poor, Average, Good, Excellent }
    public enum AttitudeRating { Poor, Average, Good, Excellent }
    public enum TeamworkRating { Poor, Average, Good, Excellent }
    public enum Punctuality { AlwaysOnTime, UsuallyOnTime, OccasionallyLate, FrequentlyLate, RarelyOnTime }

}
