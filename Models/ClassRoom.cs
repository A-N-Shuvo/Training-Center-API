using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class ClassRoom
    {
        [Key]
        public int ClassRoomId { get; set; }
        public string RoomName { get; set; }
        public int SeatCapacity { get; set; }
        public string Location { get; set; }
        public bool HasProjector { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasWhiteboard { get; set; }
        public bool HasSoundSystem { get; set; }
        public bool HasInternetAccess { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }

        public string? AdditionalFacilities { get; set; }
        public virtual ICollection<ClassRoomCourse_Junction_Table>? ClassRoomCourse_Junction_Tables { get; set; } = new List<ClassRoomCourse_Junction_Table>();
        public virtual ICollection<Batch>? Batches { get; set; }
    }
}
