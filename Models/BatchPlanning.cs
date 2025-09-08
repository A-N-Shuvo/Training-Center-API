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
        public int Year { get; set; }

        [Required]
        public int StartMonth { get; set; }  //it will happen by dropdown

        [Required]
        public int DurationMonths { get; set; } //2  or 3

        [Required]
        public int PlannedBatchCount { get; set; } // how many batches for this course or course combo?

        public string? Remarks { get; set; }

        // Navigation Property for Many-to-Many with Instructors
        [JsonIgnore]
        public virtual ICollection<BatchPlanningInstructor> BatchPlanningInstructors { get; set; } = new List<BatchPlanningInstructor>();

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
    }
}
