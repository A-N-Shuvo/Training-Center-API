using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TrainingCenter_Api.Models
{
    public class Slot
    {
        //========For Angular==========
        //[Key]
        //public int SlotID { get; set; }

        //public string TimeSlotType { get; set; } 

        //public TimeOnly StartTime { get; set; }
        //public TimeOnly EndTime { get; set; }
        //public bool IsActive { get; set; }

        //========For React=========
        [Key]
        public int SlotID { get; set; }

        public string TimeSlotType { get; set; }

        [NotMapped] // This property won't be mapped to database
        public string StartTimeString { get; set; }

        [NotMapped] // This property won't be mapped to database
        public string EndTimeString { get; set; }

        public TimeOnly StartTime
        {
            get => TimeOnly.Parse(StartTimeString);
            set => StartTimeString = value.ToString("HH:mm");
        }

        public TimeOnly EndTime
        {
            get => TimeOnly.Parse(EndTimeString);
            set => EndTimeString = value.ToString("HH:mm");
        }

        public bool IsActive { get; set; }


        //public virtual ICollection<ClassSchedule>? ClassSchedules { get; set; }
    }
}
