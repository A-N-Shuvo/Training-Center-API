using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class VisitorTransfer_Junction
    {
        [Key]
        public int VisitorTransferId { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? Notes { get; set; }
        public string? UserName { get; set; }
    }
}
