using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingCenter_Api.Models
{
    public class ClassSchedule
    {
        [Key]
        public int ClassScheduleId { get; set; }     

        [NotMapped]
        public List<int> SelectedDayIds { get; set; } = new List<int>();// This will store comma-separated day names 
        public string? SelectedDays { get; set; }
        public int SlotId { get; set; }
        public virtual Slot? Slot { get; set; }

        public DateTime ScheduleDate { get; set; }
        public bool IsActive { get; set; } = false;

        public virtual ICollection<Batch>? Batches { get; set; }

    }
}
