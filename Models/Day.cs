using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Day
    {
        [Key]
        public int DayId { get; set; }

        public string DayName { get; set; }

        public bool IsActive { get; set; }

    }
}
