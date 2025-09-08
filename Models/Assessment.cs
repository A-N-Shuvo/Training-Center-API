using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Assessment
    {
        [Key]
        public int AssessmentId { get; set; }

        //================Same===========
        public DateOnly AssessmentDate { get; set; }

        public int BatchId { get; set; } //will be same for all
        public virtual Batch? Batch { get; set; }

        public int InstructorId { get; set; } // will be same for all
        public virtual Instructor? Instructor { get; set; }



        //============unique or individual=======
        public int TraineeId { get; set; } // it will be individual or unique
        public virtual Trainee? Trainee { get; set; }

        public string AssessmentType { get; set; } // e.g., "Weekly Test", "Final Evaluation", "Behavioral"

        // Performance Metrics
        public int TheoreticalScore { get; set; } // থিওরেটিকাল স্কোর (০-১০০)
        public int PracticalScore { get; set; } // প্র্যাকটিক্যাল স্কোর (০-১০০)
        public decimal? OverallScore { get; set; } // সামগ্রিক স্কোর (স্বয়ংক্রিয়ভাবে ক্যালকুলেট করা যায়)


        // Attendance & Participation
        public int DaysPresent { get; set; }
        public int TotalDays { get; set; }       

        public decimal AttendancePercentage { get; set; } //TotalDays > 0 ? (DaysPresent * 100m / TotalDays) : 0;
        public string ParticipationLevel { get; set; } // "Low", "Medium", "High"

        // Skill Evaluation (can use enum or constants)
        public string TechnicalSkillsRating { get; set; } // "Poor", "Average", "Good", "Excellent"
        public string CommunicationSkillsRating { get; set; } //Poor, Average, Good, Excellent
        public string TeamworkRating { get; set; } //Poor, Average, Good, Excellent

        // Behavioral Assessment
        public string DisciplineRemarks { get; set; }
        public string Punctuality { get; set; } // AlwaysOnTime, UsuallyOnTime, OccasionallyLate, FrequentlyLate, RarelyOnTime
        public string AttitudeRating { get; set; } //Poor, Average, Good, Excellent

        // Trainer's Qualitative Evaluation
        public string Strengths { get; set; }
        public string Weaknesses { get; set; }
        public string ImprovementAreas { get; set; }
        public string TrainerRemarks { get; set; }

        public bool IsFinalized { get; set; } // ট্রেইনার ফাইনালাইজ করেছেন কিনা


        
        public virtual ICollection<Recommendation>? Recommendations { get; set; }
    }

    
}

