using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Visitor
    {
        [Key]
        public int VisitorId { get; set; }
        public string? VisitorNo { get; set; }
        public string VisitorName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public DateTime VisitDateTime { get; set; } 
        public string? VisitPurpose { get; set; }
        public string Address { get; set; }
        public string EducationLevel { get; set; }
        public string VisitorType { get; set; }  
        public string EmployeeComments { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public string ExpectedCourse { get; set; }
        public string VisitorSource { get; set; }
        public string? Reference { get; set; }
        public string? CompanyName { get; set; }

        public virtual ICollection<Registration>? Registrations { get; set; }
        public virtual ICollection<Admission>? Admissions { get; set; }
        public virtual ICollection<MoneyReceipt>? MoneyReceipts { get; set; }
    }
}
