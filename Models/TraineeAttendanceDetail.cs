using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class TraineeAttendanceDetail
    {
        [Key]
        public int TraineeAttendanceDetailId { get; set; }
        public int TraineeAttendanceId { get; set; }
        public virtual TraineeAttendance? TraineeAttendance { get; set; }
        public int TraineeId { get; set; }
        public virtual Trainee? Trainee { get; set; }
        public int AdmissionId { get; set; }
        public virtual Admission? Admission { get; set; }
        public int? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public bool AttendanceStatus { get; set; }
        public string? MarkedTime { get; set; }
        public string? Remarks { get; set; }
    }
}
