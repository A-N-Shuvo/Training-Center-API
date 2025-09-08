using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        public string InvoiceNo { get; set; } // Example: INV-00000001

        public DateTime CreatingDate { get; set; }

        public string InvoiceCategory { get; set; } // Course Or NonCourse Or Registration.....which will come from moneyreceipt....i will select data from dropdown and this will insert here
        public string MoneyReceiptNo { get; set; } // here will add moneyreceiptno by using coma

        public virtual ICollection<MoneyReceipt>? MoneyReceipts { get; set; } = new List<MoneyReceipt>();
        public virtual ICollection<TraineeAttendanceDetail>? TraineeAttendanceDetails { get; set; }
    }
}