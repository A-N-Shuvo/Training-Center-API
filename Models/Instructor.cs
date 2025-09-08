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

        [NotMapped] 
        public List<int> SelectedCourseIds { get; set; } = new List<int>();

        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
        public virtual ICollection<Batch>? Batches { get; set; }

        public virtual ICollection<InstructorCourse_Junction_Table>? InstructorCourse_Junction_Tables { get; set; } = new List<InstructorCourse_Junction_Table>();          
        public virtual ICollection<BatchPlanningInstructor> BatchPlanningInstructors { get; set; } = new List<BatchPlanningInstructor>();
        public virtual ICollection<TraineeAttendance>? TraineeAttendances { get; set; }
    }
}

