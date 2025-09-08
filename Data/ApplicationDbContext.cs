using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        
        public DbSet<DailySalesRecord> DailySalesRecords { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Day>Days { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<LMSResourceAccess> LMSResourceAccesses { get; set; }
        
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<CourseCombo> CourseCombos { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<AdmissionDetail> AdmissionDetails { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<TraineeAttendance> TraineeAttendances { get; set; }
        public DbSet<TraineeAttendanceDetail> TraineeAttendanceDetails { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<MoneyReceipt> MoneyReceipts { get; set; }
        public DbSet<VisitorTransfer_Junction> visitorTransfer_Junctions { get; set; }
        public DbSet<BatchTransfer_Junction> batchTransfer_Junctions { get; set; }
        public DbSet<BatchPlanning> BatchPlannings { get; set; }


        //Junction Tables are here
        public DbSet<InstructorCourse_Junction_Table> InstructorCourse_Junction_Tables { get; set; }
        public DbSet<ClassRoomCourse_Junction_Table> ClassRoomCourse_Junction_Tables { get; set; }
        public DbSet<BatchPlanningInstructor> BatchPlanningInstructors { get; set; }
        

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<VisitorEmployee>()
        //     .HasKey(ve => new { ve.VisitorId, ve.EmployeeId });


        //    modelBuilder.Entity<BatchTransfer>()
        //.HasKey(bt => new { bt.TraineeId, bt.BatchId });
        //}
    }
}