using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MoneyReceiptController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoneyReceiptController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MoneyReceipt
        [HttpGet("GetMoneyReceipts")]
        public async Task<ActionResult<IEnumerable<MoneyReceipt>>> GetMoneyReceipts()
        {
            return await _context.MoneyReceipts.ToListAsync();
        }

        // GET: api/MoneyReceipt/5
        [HttpGet("GetMoneyReceipt/{id}")]
        public async Task<ActionResult<MoneyReceipt>> GetMoneyReceipt(int id)
        {
            var moneyReceipt = await _context.MoneyReceipts.FindAsync(id);

            if (moneyReceipt == null)
            {
                return NotFound();
            }

            return moneyReceipt;
        }

        // POST: api/MoneyReceipt
        [HttpPost("InsertMoneyReceipt")]
        public async Task<ActionResult<MoneyReceipt>> PostMoneyReceipt([FromBody] MoneyReceipt moneyReceipt)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate AdmissionId exists
                if (moneyReceipt.AdmissionId.HasValue &&
                    !await _context.Admissions.AnyAsync(a => a.AdmissionId == moneyReceipt.AdmissionId))
                {
                    return BadRequest("Invalid AdmissionId");
                }

                // Generate MoneyReceiptNo (MRN-000001 format)
                var lastReceiptNo = await _context.MoneyReceipts
                    .OrderByDescending(m => m.MoneyReceiptId)
                    .Select(m => m.MoneyReceiptNo)
                    .FirstOrDefaultAsync();

                moneyReceipt.MoneyReceiptNo = GenerateNextReceiptNumber(lastReceiptNo);
                moneyReceipt.ReceiptDate = DateTime.Now;

                // Validate payment amounts
                //if (moneyReceipt.PaidAmount > moneyReceipt.PayableAmount)
                //{
                //    return BadRequest("Paid amount cannot exceed payable amount");
                //}
                if (moneyReceipt.Category != "Registration Fee")
                {
                    if (moneyReceipt.PayableAmount <= 0)
                        return BadRequest("Payable amount is required for non-registration categories.");

                    if (moneyReceipt.PaidAmount > moneyReceipt.PayableAmount)
                        return BadRequest("Paid amount cannot exceed payable amount");
                }

                if (moneyReceipt.Category == "Registration Fee" &&
                    moneyReceipt.VisitorId > 0 &&
                    moneyReceipt.IsInvoiceCreated)
                {
                    bool invoiceExists = await _context.MoneyReceipts.AnyAsync(m =>
                        m.VisitorId == moneyReceipt.VisitorId &&
                        m.Category == "Registration Fee" &&
                        m.IsInvoiceCreated);

                    if (invoiceExists)
                    {
                        return BadRequest("Invoice already created for this visitor.");
                    }
                }


                // Process invoice if needed
                if (moneyReceipt.IsFullPayment || moneyReceipt.IsInvoiceCreated)
                {
                    await ProcessInvoiceCreation(moneyReceipt);
                }



                _context.MoneyReceipts.Add(moneyReceipt);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMoneyReceipt),
                    new { id = moneyReceipt.MoneyReceiptId },
                    moneyReceipt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateNextReceiptNumber(string lastReceiptNo)
        {
            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastReceiptNo) &&
                lastReceiptNo.StartsWith("MRN-") &&
                lastReceiptNo.Length > 4)
            {
                if (int.TryParse(lastReceiptNo[4..], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"MRN-{nextNumber:D6}";
        }

        private async Task ProcessInvoiceCreation(MoneyReceipt moneyReceipt)
        {
            var allReceiptsForAdmission = await _context.MoneyReceipts
                .Where(m => m.AdmissionId == moneyReceipt.AdmissionId)
                .ToListAsync();

            var existingInvoice = allReceiptsForAdmission
                .FirstOrDefault(m => m.InvoiceId.HasValue)?
                .Invoice;

            if (existingInvoice == null)
            {
                // Create new invoice
                var lastInvoiceNo = await _context.Invoices
                    .OrderByDescending(i => i.InvoiceId)
                    .Select(i => i.InvoiceNo)
                    .FirstOrDefaultAsync();

                var invoice = new Invoice
                {
                    InvoiceNo = GenerateNextInvoiceNumber(lastInvoiceNo),
                    InvoiceCategory = moneyReceipt.Category,
                    MoneyReceiptNo = string.Join(",",
                        allReceiptsForAdmission.Select(m => m.MoneyReceiptNo)
                        .Concat(new[] { moneyReceipt.MoneyReceiptNo })
                        .Distinct()),
                    CreatingDate = DateTime.Now
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // Update all receipts with invoice ID
                foreach (var receipt in allReceiptsForAdmission)
                {
                    receipt.InvoiceId = invoice.InvoiceId;
                }
                moneyReceipt.InvoiceId = invoice.InvoiceId;
            }
            else
            {
                // Update existing invoice
                var receiptNumbers = existingInvoice.MoneyReceiptNo.Split(',')
                    .Concat(new[] { moneyReceipt.MoneyReceiptNo })
                    .Distinct()
                    .ToArray();

                existingInvoice.MoneyReceiptNo = string.Join(",", receiptNumbers);
                moneyReceipt.InvoiceId = existingInvoice.InvoiceId;
            }
        }

        private string GenerateNextInvoiceNumber(string lastInvoiceNo)
        {
            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastInvoiceNo) &&
                lastInvoiceNo.StartsWith("INV-") &&
                lastInvoiceNo.Length > 4)
            {
                if (int.TryParse(lastInvoiceNo[4..], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"INV-{nextNumber:D8}";
        }



        // PUT: api/MoneyReceipt/5
        [HttpPut("UpdateMoneyReceipt/{id}")]
        public async Task<IActionResult> PutMoneyReceipt(int id, MoneyReceipt moneyReceipt)
        {
            if (id != moneyReceipt.MoneyReceiptId)
            {
                return BadRequest();
            }

            var existingReceipt = await _context.MoneyReceipts.FindAsync(id);
            if (existingReceipt == null)
            {
                return NotFound();
            }

            // Check if we need to create/update invoice
            if ((moneyReceipt.IsFullPayment || moneyReceipt.IsInvoiceCreated) && existingReceipt.InvoiceId == null)
            {
                var invoice = new Invoice
                {
                    InvoiceCategory = moneyReceipt.Category,
                    MoneyReceiptNo = moneyReceipt.MoneyReceiptNo,
                    CreatingDate = DateTime.Now
                };

                // Generate InvoiceNo
                var lastInvoice = await _context.Invoices
                    .OrderByDescending(i => i.InvoiceId)
                    .FirstOrDefaultAsync();

                int nextInvNumber = 1;
                if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNo))
                {
                    var lastInvNumber = lastInvoice.InvoiceNo.Split('-')[1];
                    if (int.TryParse(lastInvNumber, out int invNum))
                    {
                        nextInvNumber = invNum + 1;
                    }
                }

                invoice.InvoiceNo = $"INV-{nextInvNumber:D8}";

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                moneyReceipt.InvoiceId = invoice.InvoiceId;
            }

            _context.Entry(existingReceipt).CurrentValues.SetValues(moneyReceipt);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MoneyReceiptExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/MoneyReceipt/5
        [HttpDelete("DeleteMoneyReceipt/{id}")]
        public async Task<IActionResult> DeleteMoneyReceipt(int id)
        {
            var moneyReceipt = await _context.MoneyReceipts.FindAsync(id);
            if (moneyReceipt == null)
            {
                return NotFound();
            }

            _context.MoneyReceipts.Remove(moneyReceipt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MoneyReceiptExists(int id)
        {
            return _context.MoneyReceipts.Any(e => e.MoneyReceiptId == id);
        }

        //[HttpGet("invoices-by-admission/{admissionNo}")]
        //public async Task<IActionResult> GetInvoiceNosByAdmission(string admissionNo)
        //{
        //    var invoiceNos = await _context.MoneyReceipts
        //        .Where(mr => mr.Admission != null && mr.Admission.AdmissionNo == admissionNo && mr.InvoiceId != null && mr.Invoice != null)
        //        .Select(mr => mr.Invoice.InvoiceNo)
        //        .Where(no => no != null)
        //        .Distinct()
        //        .ToListAsync();

        //    return Ok(invoiceNos);
        //}
        [HttpGet("invoices-by-admission/{admissionNo}")]
        public async Task<IActionResult> GetInvoiceNosByAdmission(string admissionNo)
        {
            var admission = await _context.Admissions
                .FirstOrDefaultAsync(a => a.AdmissionNo == admissionNo);

            if (admission == null)
                return NotFound("Admission not found");

            int visitorId = admission.VisitorId;

            // 1st: Admission-based invoices
            var admissionInvoiceNos = await _context.MoneyReceipts
                .Include(mr => mr.Invoice)
                .Where(mr => mr.AdmissionId == admission.AdmissionId && mr.InvoiceId != null)
                .Select(mr => mr.Invoice.InvoiceNo)
                .Where(no => no != null)
                .ToListAsync();

            // 2nd: Registration Fee-based invoices under same Visitor (but not linked with admission)
            var registrationFeeInvoices = await _context.MoneyReceipts
                .Include(mr => mr.Invoice)
                .Where(mr => mr.VisitorId == visitorId
                             && mr.Category == "Registration Fee"
                             && mr.InvoiceId != null)
                .Select(mr => mr.Invoice.InvoiceNo)
                .Where(no => no != null)
                .ToListAsync();

            // Combine and return unique invoice numbers
            var allInvoices = admissionInvoiceNos
                .Concat(registrationFeeInvoices)
                .Distinct()
                .ToList();

            return Ok(allInvoices);
        }
        [HttpGet("total-course-fee-by-admission/{admissionNo}")]
        public async Task<IActionResult> GetTotalCourseFeeByAdmission(string admissionNo)
        {
            var admission = await _context.Admissions
                .Include(a => a.AdmissionDetails)
                .ThenInclude(ad => ad.Batch)
                .ThenInclude(b => b.Course)
                .FirstOrDefaultAsync(a => a.AdmissionNo == admissionNo);

            if (admission == null)
                return NotFound("Admission not found");

            decimal totalFee = admission.AdmissionDetails
                .Where(ad => ad.Batch?.Course != null)
                .Sum(ad => ad.Batch.Course.CourseFee);

            return Ok(totalFee);
        }


        //[HttpGet("admission-payment-info/{admissionNo}")]
        //public async Task<IActionResult> GetAdmissionPaymentInfo(string admissionNo)
        //{
        //    var admission = await _context.Admissions
        //        .Include(a => a.AdmissionDetails)
        //            .ThenInclude(ad => ad.Batch)
        //                .ThenInclude(b => b.Course)
        //        .FirstOrDefaultAsync(a => a.AdmissionNo == admissionNo);

        //    if (admission == null)
        //        return NotFound("Admission not found");

        //    decimal totalAmount = admission.AdmissionDetails
        //        .Where(ad => ad.Batch != null && ad.Batch.Course != null)
        //        .Sum(ad => ad.Batch.Course.CourseFee);

        //    decimal totalPaid = await _context.MoneyReceipts
        //        .Where(m => m.AdmissionId == admission.AdmissionId && m.Category == "Course")
        //        .SumAsync(m => m.PaidAmount);

        //    return Ok(new
        //    {
        //        totalAmount,
        //        totalPaid,
        //        payableAmount = totalAmount - totalPaid
        //    });
        //}


        [HttpGet("admission-payment-info/{admissionNo}")]
        public async Task<IActionResult> GetAdmissionPaymentInfo(string admissionNo)
        {
            var admission = await _context.Admissions
                .Include(a => a.AdmissionDetails)
                    .ThenInclude(ad => ad.Batch)
                        .ThenInclude(b => b.Course)
                .FirstOrDefaultAsync(a => a.AdmissionNo == admissionNo);

            if (admission == null)
                return NotFound("Admission not found");

            int visitorId = admission.VisitorId;

            // Total course fee
            decimal totalAmount = admission.AdmissionDetails
                .Where(ad => ad.Batch != null && ad.Batch.Course != null)
                .Sum(ad => ad.Batch.Course.CourseFee);

            // Total paid course receipts
            decimal coursePaid = await _context.MoneyReceipts
                .Where(m => m.AdmissionId == admission.AdmissionId && m.Category == "Course")
                .SumAsync(m => m.PaidAmount);

            // Registration Fee paid under this visitor
            decimal registrationFeePaid = await _context.MoneyReceipts
                .Where(m => m.VisitorId == visitorId && m.Category == "Registration Fee")
                .SumAsync(m => m.PaidAmount);

            // Calculate actual payable
            decimal payableAmount = totalAmount - coursePaid - registrationFeePaid;

            return Ok(new
            {
                totalAmount,
                coursePaid,
                registrationFeePaid,
                payableAmount = payableAmount > 0 ? payableAmount : 0
            });
        }


    }
}