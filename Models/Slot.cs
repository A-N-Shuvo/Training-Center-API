using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TrainingCenter_Api.Models
{
    public class Slot
    {
        [Key]
        public int SlotID { get; set; }

        public string TimeSlotType { get; set; }

        [NotMapped] 
        public string StartTimeString { get; set; }

        [NotMapped] 
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

    }
}
