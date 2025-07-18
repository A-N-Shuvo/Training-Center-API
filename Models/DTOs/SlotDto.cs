namespace TrainingCenter_Api.Models.DTOs
{
    public class SlotDto
    {
        public int SlotID { get; set; }
        public string TimeSlotType { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
