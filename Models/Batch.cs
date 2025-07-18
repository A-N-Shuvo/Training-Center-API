using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class Batch
    {
        [Key]
        public int BatchId { get; set; }

        public string BatchName { get; set; }

        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string BatchType { get; set; } // Regular, Weekend, Online

        public int InstructorId { get; set; }
        public virtual Instructor? Instructor { get; set; }

        public int ClassRoomId { get; set; }
        public virtual ClassRoom? ClassRoom { get; set; }

        public bool IsActive { get; set; } = true;

        public string? Remarks { get; set; }

        // ✅ One-to-Many Relationship with ClassSchedule
        public virtual ICollection<ClassSchedule>? ClassSchedules { get; set; } = new List<ClassSchedule>();

        public virtual ICollection<TraineeAttendance>? TraineeAttendances { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }

        public virtual ICollection<AdmissionDetail>? AdmissionDetails { get; set; }

        public virtual ICollection<Trainee>? Trainees { get; set; }

        public virtual ICollection<LMSResourceAccess>? LMSResources { get; set; }

    }
}
