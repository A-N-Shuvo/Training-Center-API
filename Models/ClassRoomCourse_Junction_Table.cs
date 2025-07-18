using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class ClassRoomCourse_Junction_Table
    {
        [Key]
        public int ClassRoomCourseId { get; set; }

        [ForeignKey("ClassRoom")]
        public int ClassRoomId { get; set; }
        public virtual ClassRoom? ClassRoom { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
