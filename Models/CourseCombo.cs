using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class CourseCombo
    {
        [Key]
        public int CourseComboId { get; set; }
        public string? ComboName { get; set; }

        public string? SelectedCourse { get; set; }

        [NotMapped]
        public List<int> SelectedCourseIds { get; set; } = new List<int>();
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }


        public virtual ICollection<BatchPlanning>? BatchPlannings { get; set; }

        public virtual ICollection<Registration>? Registrations { get; set; }

    }
}
