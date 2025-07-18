namespace TrainingCenter_Api.Models
{
    public class DailySalesRecord
    {
        public int DailySalesRecordId { get; set; } // Auto-generated ID

        public int EmployeeId { get; set; } //Sales Agent
        public virtual Employee? Employee { get; set; }

        // === Date & Agent Info ===
        public DateTime Date { get; set; } = DateTime.Now;

        // === Cold Calling Stats ===
        public int ColdCallsMade { get; set; } // Today's cold calls (e.g., 22)
        public int MeetingsScheduled { get; set; } // Scheduled via calls (e.g., 5)
        public int MeetingsConducted { get; set; } // Done today (e.g., 0)
        public string VisitorNo { get; set; } // Comma-separated (e.g., "3466,3468,3457")

        // === Walk-ins & Evaluations ===
        public int WalkInsAttended { get; set; } // e.g., 1
        public string WalkInVisitorNo { get; set; } // e.g., "3457"
        public int EvaluationsAttended { get; set; } // e.g., 0

        // === Corporate Visits ===
        public int CorporateVisitsScheduled { get; set; } // e.g., 0
        public int CorporateVisitsConducted { get; set; } // e.g., 0

        // === Admissions & Enrollment ===
        public int NewRegistrations { get; set; } // e.g., 0
        public int Enrollments { get; set; } // Paid admissions (e.g., 0)

        // === Financial Updates ===
        public decimal NewCollections { get; set; } // Today's payment (e.g., 18500)

        public decimal DueCollections { get; set; } // Pending payments (e.g., 0)

        //public decimal TotalCollections { get; set; } // Total till date (e.g., 59500)

        public string? Remarks { get; set; } 
    }
}
