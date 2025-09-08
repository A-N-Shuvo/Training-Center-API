using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Admission
    {
        [Key]
        public int AdmissionId { get; set; }
     
        public string? AdmissionNo { get; set; }

        [Required]
        public int VisitorId { get; set; }
        public virtual Visitor? Visitors { get; set; }

        public string? OrganizationName { get; set; }

        public int? OfferId { get; set; }
        public Offer? Offer { get; set; }

        public decimal? DiscountAmount { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }
        public string? Remarks { get; set; }

        //public virtual ICollection<Invoice>? Invoices { get; set; }
        public List<AdmissionDetail> AdmissionDetails { get; set; } = new List<AdmissionDetail>();


        public virtual ICollection<MoneyReceipt>? moneyReceipts { get; set; }

        public virtual ICollection<TraineeAttendanceDetail>? TraineeAttendanceDetails { get; set; }
    }
}
