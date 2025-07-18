using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrainingCenter_Api.Data;

namespace TrainingCenter_Api.Models
{
    public class AdmissionDetail
    {
        [Key]
        public int AdmissionDetailsId { get; set; }

        [Required]
        public int AdmissionId { get; set; }
        public virtual Admission? Admission { get; set; }

        [Required]
        public int RegistrationId { get; set; }
        public virtual Registration? Registration { get; set; }


        [Required]
        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }
    }  
}
