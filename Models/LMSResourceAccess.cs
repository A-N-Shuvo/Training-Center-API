using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class LMSResourceAccess
    {
        [Key]
        public int ResourceId { get; set; }

        [Required]
        [Display(Name = "Batch")]
        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }

        [Required]
        [Display(Name = "Resource Type")]
        public string ResourceType { get; set; } // Video, PDF, Slide, etc.

        [Required]
        [Display(Name = "Resource Title")]
        public string ResourceTitle { get; set; }

        [Required]
        [Display(Name = "File URL or Path")]
        public string ResourceUrl { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Display(Name = "Uploaded By")]
        public int UploadedByEmployeeId { get; set; }
        public virtual Employee? UploadedByEmployee { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

    }
}
