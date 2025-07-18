using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Designation
    {
        [Key]
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string? JobRoles { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }

        public virtual ICollection<Employee>? Employees { get; set; }

    }
}
