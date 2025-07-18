using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class Instructor
    {
        [Key]
        public int InstructorId { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        //public string? Specialization { get; set; }
        [NotMapped] 
        public List<int> SelectedCourseIds { get; set; } = new List<int>();// This will store comma-separated course names or IDs

        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
        public virtual ICollection<Batch>? Batches { get; set; }

        // Many-to-Many: Instructor ↔ Course (via InstructorCourse)
        public virtual ICollection<InstructorCourse_Junction_Table>? InstructorCourse_Junction_Tables { get; set; } = new List<InstructorCourse_Junction_Table>();  //It's very important

        //public virtual ICollection<Assessment>? Assessments { get; set; }
        //public virtual ICollection<BatchPlanningInstructor>? BatchPlanningInstructors { get; set; }

        // Reverse navigation
        public virtual ICollection<BatchPlanningInstructor> BatchPlanningInstructors { get; set; } = new List<BatchPlanningInstructor>();
    }
}
