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

                // Validate AdmissionId exists if provided
                if (moneyReceipt.AdmissionId.HasValue &&
                    !await _context.Admissions.AnyAsync(a => a.AdmissionId == moneyReceipt.AdmissionId))
                {
                    return BadRequest("Invalid AdmissionId");
                }

                // Generate MoneyReceiptNo (MRN-000001 format)
                moneyReceipt.MoneyReceiptNo = GenerateNextReceiptNumber(await GetLastReceiptNo());
                moneyReceipt.ReceiptDate = DateTime.Now;

                // Validate payment amounts for non-registration categories
                if (moneyReceipt.Category != "Registration Fee")
                {
                    if (moneyReceipt.PayableAmount <= 0)
                        return BadRequest("Payable amount is required for non-registration categories.");

                    if (moneyReceipt.PaidAmount > moneyReceipt.PayableAmount)
                        return BadRequest("Paid amount cannot exceed payable amount");
                }

                // Check if an invoice already exists for this Visitor
                var existingInvoice = await _context.MoneyReceipts
                    .Include(m => m.Invoice)
                    .Where(m => m.VisitorId == moneyReceipt.VisitorId && m.InvoiceId != null)
                    .Select(m => m.Invoice)
                    .FirstOrDefaultAsync();

                // VALIDATION: Prevent duplicate invoice creation
                if ((moneyReceipt.IsInvoiceCreated || moneyReceipt.IsFullPayment) && existingInvoice != null)
                {
                    return BadRequest("Invoice already exists for this visitor. Cannot create another invoice.");
                }

                // If invoice exists, add this receipt to it
                if (existingInvoice != null)
                {
                    var receiptNumbers = existingInvoice.MoneyReceiptNo.Split(',')
                        .Append(moneyReceipt.MoneyReceiptNo)
                        .Distinct()
                        .ToArray();

                    existingInvoice.MoneyReceiptNo = string.Join(",", receiptNumbers);
                    moneyReceipt.InvoiceId = existingInvoice.InvoiceId;
                }
                // If no invoice exists and invoice creation is requested
                else if (moneyReceipt.IsInvoiceCreated || moneyReceipt.IsFullPayment)
                {
                    // Get all existing money receipts for this Visitor (if any)
                    var allReceiptsForVisitor = await _context.MoneyReceipts
                        .Where(m => m.VisitorId == moneyReceipt.VisitorId)
                        .ToListAsync();

                    // Generate new invoice
                    var invoice = new Invoice
                    {
                        InvoiceNo = GenerateNextInvoiceNumber(await GetLastInvoiceNo()),
                        InvoiceCategory = moneyReceipt.Category,
                        MoneyReceiptNo = string.Join(",",
                            allReceiptsForVisitor.Select(m => m.MoneyReceiptNo)
                            .Concat(new[] { moneyReceipt.MoneyReceiptNo })
                            .Distinct()),
                        CreatingDate = DateTime.Now
                    };

                    _context.Invoices.Add(invoice);
                    await _context.SaveChangesAsync();

                    // Update all existing receipts with the new InvoiceId
                    foreach (var receipt in allReceiptsForVisitor)
                    {
                        receipt.InvoiceId = invoice.InvoiceId;
                        _context.Entry(receipt).State = EntityState.Modified;
                    }
                    moneyReceipt.InvoiceId = invoice.InvoiceId;
                }

                // Save the money receipt
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

        // Helper method to get last receipt number
        private async Task<string> GetLastReceiptNo()
        {
            return await _context.MoneyReceipts
                .OrderByDescending(m => m.MoneyReceiptId)
                .Select(m => m.MoneyReceiptNo)
                .FirstOrDefaultAsync();
        }

        // Helper method to get last invoice number
        private async Task<string> GetLastInvoiceNo()
        {
            return await _context.Invoices
                .OrderByDescending(i => i.InvoiceId)
                .Select(i => i.InvoiceNo)
                .FirstOrDefaultAsync();
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
            // 1. Check if any invoice exists for this Visitor (regardless of Category)
            var existingInvoice = await _context.MoneyReceipts
                .Include(m => m.Invoice)
                .Where(m => m.VisitorId == moneyReceipt.VisitorId && m.InvoiceId != null)
                .Select(m => m.Invoice)
                .FirstOrDefaultAsync();

            // 2. If invoice exists, add the new money receipt to it
            if (existingInvoice != null)
            {
                var receiptNumbers = existingInvoice.MoneyReceiptNo.Split(',')
                    .Append(moneyReceipt.MoneyReceiptNo)
                    .Distinct()
                    .ToArray();

                existingInvoice.MoneyReceiptNo = string.Join(",", receiptNumbers);
                moneyReceipt.InvoiceId = existingInvoice.InvoiceId;
                return; // Exit after updating existing invoice
            }

            // 3. If no invoice exists, create a new one (if requested)
            if (moneyReceipt.IsInvoiceCreated || moneyReceipt.IsFullPayment)
            {
                var lastInvoiceNo = await _context.Invoices
                    .OrderByDescending(i => i.InvoiceId)
                    .Select(i => i.InvoiceNo)
                    .FirstOrDefaultAsync();

                var invoice = new Invoice
                {
                    InvoiceNo = GenerateNextInvoiceNumber(lastInvoiceNo),
                    InvoiceCategory = moneyReceipt.Category, // First payment's category
                    MoneyReceiptNo = moneyReceipt.MoneyReceiptNo,
                    CreatingDate = DateTime.Now
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                moneyReceipt.InvoiceId = invoice.InvoiceId;
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



        [HttpPut("UpdateMoneyReceipt/{id}")]
        public async Task<IActionResult> PutMoneyReceipt(int id, MoneyReceipt moneyReceipt)
        {
            try
            {
                if (id != moneyReceipt.MoneyReceiptId)
                {
                    return BadRequest("ID mismatch");
                }

                var existingReceipt = await _context.MoneyReceipts
                    .Include(m => m.Invoice)
                    .FirstOrDefaultAsync(m => m.MoneyReceiptId == id);

                if (existingReceipt == null)
                {
                    return NotFound("Money receipt not found");
                }

                // Save the original MoneyReceiptNo before updating
                string originalReceiptNo = existingReceipt.MoneyReceiptNo;
                DateTime originalReceiptDate = existingReceipt.ReceiptDate;

                // Validate model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate AdmissionId exists if provided
                if (moneyReceipt.AdmissionId.HasValue &&
                    !await _context.Admissions.AnyAsync(a => a.AdmissionId == moneyReceipt.AdmissionId))
                {
                    return BadRequest("Invalid AdmissionId");
                }

                // Validate payment amounts for non-registration categories
                if (moneyReceipt.Category != "Registration Fee")
                {
                    if (moneyReceipt.PayableAmount <= 0)
                        return BadRequest("Payable amount is required for non-registration categories.");

                    if (moneyReceipt.PaidAmount > moneyReceipt.PayableAmount)
                        return BadRequest("Paid amount cannot exceed payable amount");
                }

                // Check if trying to create invoice for existing receipt
                if ((moneyReceipt.IsInvoiceCreated || moneyReceipt.IsFullPayment) &&
                    existingReceipt.InvoiceId == null)
                {
                    // Check if visitor already has an invoice
                    var visitorInvoice = await _context.MoneyReceipts
                        .Include(m => m.Invoice)
                        .Where(m => m.VisitorId == moneyReceipt.VisitorId &&
                                   m.MoneyReceiptId != id &&
                                   m.InvoiceId != null)
                        .Select(m => m.Invoice)
                        .FirstOrDefaultAsync();

                    if (visitorInvoice != null)
                    {
                        // Add to existing invoice
                        var receiptNumbers = visitorInvoice.MoneyReceiptNo.Split(',')
                            .Append(originalReceiptNo)  // Use the original receipt number
                            .Distinct()
                            .ToArray();

                        visitorInvoice.MoneyReceiptNo = string.Join(",", receiptNumbers);
                        existingReceipt.InvoiceId = visitorInvoice.InvoiceId;
                    }
                    else
                    {
                        // Create new invoice
                        var invoice = new Invoice
                        {
                            InvoiceNo = GenerateNextInvoiceNumber(await GetLastInvoiceNo()),
                            InvoiceCategory = moneyReceipt.Category,
                            MoneyReceiptNo = originalReceiptNo,  // Use the original receipt number
                            CreatingDate = DateTime.Now
                        };

                        _context.Invoices.Add(invoice);
                        await _context.SaveChangesAsync();
                        existingReceipt.InvoiceId = invoice.InvoiceId;
                    }
                }

                // Update all fields except MoneyReceiptNo and ReceiptDate
                _context.Entry(existingReceipt).CurrentValues.SetValues(moneyReceipt);

                // Restore the original values for these fields
                existingReceipt.MoneyReceiptNo = originalReceiptNo;
                existingReceipt.ReceiptDate = originalReceiptDate;

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
        

        [HttpGet("visitor-payment-summary/{visitorId}")]
        public async Task<IActionResult> GetVisitorPaymentSummary(int visitorId)
        {
            try
            {
                // 1. Calculate total course fees after discounts
                var admissions = await _context.Admissions
                    .Include(a => a.AdmissionDetails)
                        .ThenInclude(ad => ad.Batch)
                            .ThenInclude(b => b.Course)
                    .Include(a => a.Offer)
                    .Where(a => a.VisitorId == visitorId)
                    .ToListAsync();

                decimal totalAfterDiscounts = admissions.Sum(a => {
                    decimal rawTotal = a.AdmissionDetails.Sum(ad => ad.Batch?.Course?.CourseFee ?? 0);
                    decimal afterOffer = a.Offer != null ? rawTotal * (100 - a.Offer.DiscountPercentage) / 100 : rawTotal;
                    return afterOffer - (a.DiscountAmount ?? 0);
                });

                // 2. Get ALL payments (both Course and Registration Fee)
                decimal totalPaid = await _context.MoneyReceipts
                    .Where(mr => mr.VisitorId == visitorId)
                    .SumAsync(mr => mr.PaidAmount);

                // 3. Calculate remaining payable
                decimal remainingPayable = totalAfterDiscounts - totalPaid;

                return Ok(new
                {
                    TotalAfterDiscounts = totalAfterDiscounts,
                    TotalPaid = totalPaid,
                    RemainingPayable = remainingPayable > 0 ? remainingPayable : 0
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }    



    }
}