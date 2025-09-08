using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraineeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TraineeController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: api/Trainee
        [HttpGet("GetTrainees")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainees()
        {
            var trainees = await _context.Trainees
                .Include(t => t.Registration)
                .Select(t => new
                {
                    t.TraineeId,
                    TraineeName = t.Registration.TraineeName,
                    t.TraineeIDNo,
                    RegistrationNo = t.Registration.RegistrationNo
                })
                .ToListAsync();

            return Ok(trainees);
        }

        // GET: api/Trainee/5
        [HttpGet("GetTraineeDetails/{id}")]
        public async Task<ActionResult<object>> GetTraineeDetails(int id)
        {
            var trainee = await _context.Trainees
                .Include(t => t.Registration)
                .Include(t => t.Admission)
                    .ThenInclude(a => a.moneyReceipts)
                        .ThenInclude(mr => mr.Invoice)
                .Where(t => t.TraineeId == id)
                .Select(t => new
                {
                    TraineeId = t.TraineeId,
                    TraineeName = t.Registration.TraineeName,
                    TraineeIDNo = t.TraineeIDNo,
                    RegistrationNo = t.Registration.RegistrationNo,
                    AdmissionNo = t.Admission.AdmissionNo,
                    Invoices = t.Admission.moneyReceipts
                        .Where(mr => mr.Invoice != null)
                        .Select(mr => new
                        {
                            InvoiceNo = mr.Invoice.InvoiceNo,
                            InvoiceDate = mr.Invoice.CreatingDate,
                            AmountPaid = mr.PaidAmount
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (trainee == null)
            {
                return NotFound();
            }

            return Ok(trainee);
        }
    }
}