using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Trainee
    {
        [Key]
        public int TraineeId { get; set; }

        [Display(Name = "Trainee Id No")]
        public string TraineeIDNo { get; set; } //auto generate 
        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }

        public int RegistrationId { get; set; }
        public virtual Registration? Registration { get; set; }
        
        
        public int AdmissionId { get; set; }
        public virtual Admission? Admission { get; set; }


        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        public virtual ICollection<TraineeAttendanceDetail>? TraineeAttendanceDetails { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }
        public virtual ICollection<Recommendation>? Recommendations { get; set; }
    }
}
