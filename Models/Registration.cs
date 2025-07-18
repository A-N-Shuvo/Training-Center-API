using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrainingCenter_Api.Models
{
    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }  

        public string? RegistraionNo { get; set; }

        public int VisitorId { get; set; }
        public virtual Visitor? Visitor { get; set; }

        public string TraineeName { get; set; }

        public DateTime RegistrationDate { get; set; }

     

        public int? CourseId { get; set; }
        public virtual Course? Course { get; set; }

        //======= For Course combo ======

        public int? CourseComboId { get; set; }
        public virtual CourseCombo? CourseCombo { get; set; }
      

        public string Gender { get; set; }

        public string Nationality { get; set; }

        public string Religion { get; set; }


        public DateOnly DateOfBirth { get; set; }


        public DateOnly OriginatDateofBirth { get; set; }

        public string MaritalStatus { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public string ContactNo { get; set; }
        public string? EmergencyContactNo { get; set; }


        public string EmailAddress { get; set; }



        public string? BloodGroup { get; set; }


        public string? ImagePath { get; set; }
        [NotMapped] 
        public IFormFile? ImageFile { get; set; }

        public string? DocumentPath { get; set; }

        [NotMapped]
        public IFormFile? DocumentFile { get; set; }


        public string BirthOrNIDNo { get; set; }


        public string PresentAddress { get; set; }

        public string PermanentAddress { get; set; }

        public string? HighestEducation { get; set; }

        public string? InstitutionName { get; set; }


        public string Reference { get; set; } //Executive name
        public string? Remarks { get; set; }

        public virtual ICollection<Trainee>? Trainees { get; set; }
        public virtual ICollection<AdmissionDetail>? AdmissionDetails { get; set; }
    }
}
