using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class MoneyReceipt
    {
        [Key]
        public int MoneyReceiptId { get; set; }

        public string? MoneyReceiptNo { get; set; } // MRN-000001

        public DateTime ReceiptDate { get; set; }

        public string Category { get; set; }  // Course, NonCourse, Registration Fee
        public int? AdmissionId { get; set; }
        public virtual Admission? Admission { get; set; }
        public int? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        public int? VisitorId { get; set; }
        public virtual Visitor? Visitor { get; set; } // All Registration No will get up in a box according to Visitor name

        public string PaymentMode { get; set; }    // Cash, Cheque, MFS, Card

        //====For Cheque Details
        public string? ChequeNo { get; set; }
        public string? BankName { get; set; } // It will work for both Cheque and Debit or Credit Card

        //====For MFS Details
        public string? MFSName { get; set; } // Bkash, Rocket, Nagad
        public string? TransactionNo { get; set; }

        //====For Card Details
        public string? DebitOrCreditCardNo { get; set; }

        public bool IsFullPayment { get; set; }

        public bool IsInvoiceCreated { get; set; }

        // Suppose Total amount 10000...1st time payable amount 10000,,,Paid Amount 8000,,,Due Amount 2000.....2nd time Total amount 10000,,,, Payable amount 2000,,,,Paid Amount 2000,,,, Due amount 000

        public decimal PayableAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }

        public string? CreatedBy { get; set; }
        public string? Remarks { get; set; }
    }
}
