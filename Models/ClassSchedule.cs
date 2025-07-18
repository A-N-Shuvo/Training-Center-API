using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class ClassSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public int BatchId { get; set; }
        public virtual Batch? Batch { get; set; }

        public int DayId { get; set; }
        public virtual Day? Day { get; set; }

        public int SlotId { get; set; }
        public virtual Slot? Slot { get; set; }

        public DateTime ScheduleDate { get; set; }

        public bool IsHoliday { get; set; }

    }
}
