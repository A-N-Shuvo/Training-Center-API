using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class BatchPlanningInstructor
    {
        [Key]
        public int Id { get; set; }

        public int BatchPlanningId { get; set; }
        public virtual BatchPlanning BatchPlanning { get; set; } = null!;

        public int InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; } = null!;

    }
}
