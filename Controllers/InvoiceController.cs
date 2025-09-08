using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // InvoiceController.cs
        [HttpGet("GetAllInvoices")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoicesForDropdown()
        {
            var invoices = await _context.Invoices
                .Select(i => new
                {
                    i.InvoiceId,
                    i.InvoiceNo,
                    i.CreatingDate
                })
                .ToListAsync();

            return Ok(invoices);
        }

        // GET: api/Invoice/5
        [HttpGet("GetInvoiceDetails/{id}")]
        public async Task<ActionResult<object>> GetInvoiceDetails(int id)
        {
            var invoice = await _context.Invoices
                .Where(i => i.InvoiceId == id)
                .Select(i => new
                {
                    InvoiceNo = i.InvoiceNo,
                    CreatingDate = i.CreatingDate,
                    InvoiceCategory = i.InvoiceCategory,
                    MoneyReceipts = i.MoneyReceipts.Select(mr => new
                    {
                        mr.MoneyReceiptNo,
                        mr.PaidAmount,
                        AdmissionNo = mr.Admission != null ? mr.Admission.AdmissionNo : null,
                        VisitorName = mr.Admission != null ? mr.Admission.Visitors.VisitorName :
                                    mr.Visitor != null ? mr.Visitor.VisitorName : null
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }
    }
}