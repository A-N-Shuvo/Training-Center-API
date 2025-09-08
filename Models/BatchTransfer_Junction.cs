using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class BatchTransfer_Junction
    {
        [Key]
        public int BatchTransferId { get; set; }
        public int TraineeId { get; set; }
        public Trainee? Trainee { get; set; }
        public int BatchId { get; set; }
        public Batch? Batch { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public DateOnly? TransferDate { get; set; }
    }
}
