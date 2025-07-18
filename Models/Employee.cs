using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string? EmployeeIDNo { get; set; }
        public string EmployeeName { get; set; }
        public int DesignationId { get; set; }
        public virtual Designation? Designation { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public string ContactNo { get; set; }
        public DateOnly DOB { get; set; }
        public DateTime JoiningDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string EmailAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string PresentAddress { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public int BirthOrNIDNo { get; set; }
        public bool IsAvailable { get; set; } // string থেকে bool এ পরিবর্তন করুন
        public bool IsWillingToSell { get; set; }      
        public string? ImagePath { get; set; }
        public string? DocumentPath { get; set; }

        [NotMapped] // এই এনোটেশন দিয়ে EF কে জানান এটি ডাটাবেসে সংরক্ষণ করবে না
        public IFormFile? ImageFile { get; set; }
        [NotMapped]
        public IFormFile? DocumentFile { get; set; }
        public string? Remarks { get; set; }
        public virtual ICollection<Visitor>? Visitors { get; set; }
        public virtual ICollection<Instructor>? Instructor { get; set; }

        public virtual ICollection<DailySalesRecord>? DailySalesRecords { get; set; }
    }
}
