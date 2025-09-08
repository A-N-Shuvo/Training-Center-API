using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class Batch
    {
        [Key]
        public int BatchId { get; set; }

        public string? BatchName { get; set; }

        public int CourseId { get; set; }
        public virtual Course? Course { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string BatchType { get; set; } // Regular, Weekend, Online

        public int InstructorId { get; set; }       
        public virtual Instructor? Instructor { get; set; }
        [NotMapped]
        public List<int> PreviousInstructorIds { get; set; } = new List<int>();

        public string? PreviousInstructorIdsString
        {
            get => PreviousInstructorIds.Any() ? string.Join(",", PreviousInstructorIds) : null;
            set => PreviousInstructorIds = !string.IsNullOrEmpty(value) ?
                value.Split(',').Select(int.Parse).ToList() : new List<int>();
        }
        public int ClassRoomId { get; set; }
        public virtual ClassRoom? ClassRoom { get; set; }

        [NotMapped]
        public List<int> SelectedScheduleIds { get; set; } = new List<int>();
        public string? SelectedClassSchedules { get; set; }


        public bool IsActive { get; set; } = true;

        public string? Remarks { get; set; }

        public virtual ICollection<ClassSchedule>? ClassSchedules { get; set; }

        public virtual ICollection<TraineeAttendance>? TraineeAttendances { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }

        public virtual ICollection<AdmissionDetail>? AdmissionDetails { get; set; }

        public virtual ICollection<Trainee>? Trainees { get; set; }

        public virtual ICollection<LMSResourceAccess>? LMSResources { get; set; }


    }
}
