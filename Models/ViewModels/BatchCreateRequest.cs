namespace TrainingCenter_Api.Models.ViewModels
{
    public class BatchCreateRequest
    {
        public string BatchName { get; set; }
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string BatchType { get; set; }
        public int InstructorId { get; set; }
        public int ClassRoomId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        // ✅ এই লিস্টের মাধ্যমে ClassScheduleId গুলো রিসিভ হবে
        public List<int> SelectedClassScheduleIds { get; set; } = new();

    }
}
