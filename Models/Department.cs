using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }

    }
}
