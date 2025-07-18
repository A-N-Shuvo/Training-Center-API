using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class TraineeAttendance
    {
        [Key]
        public int TraineeAttendanceId { get; set; }

        // Required relationships
        [Required]
        public int TraineeId { get; set; }
        public virtual Trainee? Trainee { get; set; }

        [Required]
        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }

        // Instructor will automatically come from Batch relationship
        // No separate field needed as we'll always use Batch.Instructor

        // Attendance details
        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public string Status { get; set; } = "Present"; // Default value

        // Financial tracking
        public string InvoiceNo { get; set; } = "No Invoice";

        // Admission reference
        [ForeignKey("Admission")]
        public int AdmissionId { get; set; }
        public virtual Admission? Admission { get; set; }

        // Tracking fields
        public DateTime MarkedTime { get; set; } = DateTime.Now;

        public string? Remarks { get; set; }

        // Computed property for easy access to instructor name
        [NotMapped]
        public string InstructorName => Batch?.Instructor?.Employee?.EmployeeName ?? "Not Assigned";
    }
}
