using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainingCenter_Api.Models
{
    public class BatchPlanning
    {
        [Key]
        public int BatchPlanningId { get; set; }

        // Reference to either Course or CourseCombo
        public int? CourseId { get; set; }
        [JsonIgnore]
        public virtual Course? Course { get; set; }

        public int? CourseComboId { get; set; }
        [JsonIgnore]
        public virtual CourseCombo? CourseCombo { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
        public int Year { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Start month must be between 1 and 12")]
        public int StartMonth { get; set; }

        [Required]
        [Range(1, 24, ErrorMessage = "Duration must be between 1 and 24 months")]
        public int DurationMonths { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Planned batch count must be between 1 and 20")]
        public int PlannedBatchCount { get; set; }

        public string? Remarks { get; set; }

        // Navigation Property for Many-to-Many with Instructors
        [JsonIgnore]
        public virtual ICollection<BatchPlanningInstructor> BatchPlanningInstructors { get; set; } = new List<BatchPlanningInstructor>();

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
    }
}
