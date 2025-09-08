using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class TraineeAttendance
    {
        [Key]
        public int TraineeAttendanceId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }
        public int InstructorId { get; set; }
        public virtual Instructor? Instructor { get; set; }
        public virtual ICollection<TraineeAttendanceDetail>? TraineeAttendanceDetails { get; set; }
    }
}
