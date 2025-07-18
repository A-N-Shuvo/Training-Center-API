using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Offer
    {
        [Key]
        public int OfferId { get; set; }
        public string OfferName { get; set; }
        public string Description { get; set; }
        public string OfferType { get; set; }      // Seasonal, Occasional, PaymentBased
        public string SeasonOrOccasion { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string PaymentCondition { get; set; }

        public bool IsActive { get; set; } = true;
        public string? Remarks { get; set; }

        public ICollection<Admission>? Admissions { get; set; }


    }
}
