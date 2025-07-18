using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrainingCenter_Api.Models
{
    public class InstructorCourse_Junction_Table
    {
        [Key]
        public int InstructorCourseId { get; set; }

        [ForeignKey("Instructor")]
        public int InstructorId { get; set; }
        [JsonIgnore]
        public virtual Instructor? Instructor { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        [JsonIgnore]
        public virtual Course? Course { get; set; }

        public DateTime AssignmentDate { get; set; } = DateTime.Now;

        public bool IsPrimaryInstructor { get; set; } = false;
    }
}
