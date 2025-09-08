using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string ShortCode { get; set; }
        public string TotalHours { get; set; }
        public decimal CourseFee { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<Batch>? Batches { get; set; }
        public virtual ICollection<Registration>? Registrations { get; set; }
        public virtual ICollection<InstructorCourse_Junction_Table>? InstructorCourse_Junction_Tables { get; set; } = new List<InstructorCourse_Junction_Table> ();
        public virtual ICollection<ClassRoomCourse_Junction_Table>? ClassRoomCourse_Junction_Tables { get; set; } = new List<ClassRoomCourse_Junction_Table>();

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        public virtual ICollection<BatchPlanning>? BatchPlannings { get; set; }
    }
}
