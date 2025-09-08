using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Migrations;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorTransferController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VisitorTransferController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/VisitorTransfer
        [HttpGet("GetVisitorTransfers")]
        public async Task<IActionResult> GetVisitorTransfers()
        {
            var transfers = await _context.visitorTransfer_Junctions
                .Include(v => v.Visitor)
                .Include(e => e.Employee)
                .OrderByDescending(t => t.TransferDate)
                .Select(t => new
                {
                    t.VisitorTransferId,
                    t.VisitorId,
                    VisitorName = t.Visitor.VisitorName,
                    t.EmployeeId,
                    EmployeeName = t.Employee.EmployeeName,
                    t.TransferDate,
                    t.Notes,
                    t.UserName
                })
                .ToListAsync();

            return Ok(transfers);
        }


        // GET: api/VisitorTransfer/{id}
        [HttpGet("GetVisitorTransfer/{id}")]
        public async Task<IActionResult> GetVisitorTransfer(int id)
        {
            var transfer = await _context.visitorTransfer_Junctions
                .Include(v => v.Visitor)
                .Include(e => e.Employee)
                .Where(t => t.VisitorTransferId == id)
                .Select(t => new
                {
                    t.VisitorTransferId,
                    t.VisitorId,
                    VisitorName = t.Visitor.VisitorName,
                    t.EmployeeId,
                    EmployeeName = t.Employee.EmployeeName,
                    t.TransferDate,
                    t.Notes,
                    t.UserName
                })
                .FirstOrDefaultAsync();

            if (transfer == null)
                return NotFound();

            return Ok(transfer);
        }


        [HttpGet("ByEmployee/{employeeId}")]
        public async Task<IActionResult> GetVisitorsByEmployeeId(int employeeId)
        {
            var visitors = await _context.Visitors
                .Where(v => v.EmployeeId == employeeId)
                .Select(v => new
                {
                    v.VisitorId,
                    v.VisitorName,
                    v.EmployeeId
                })
                .ToListAsync();

            return Ok(visitors);
        }

        [HttpGet("AssignedVisitorsByEmployee/{employeeId}")]
        public async Task<IActionResult> GetAssignedVisitorsByEmployeeId(int employeeId)
        {
            var assignedVisitors = await _context.visitorTransfer_Junctions
                .Include(vt => vt.Visitor)
                .Where(vt => vt.EmployeeId == employeeId)
                .Select(vt => new
                {
                    vt.VisitorTransferId,
                    vt.Visitor.VisitorNo,
                    VisitorName = vt.Visitor.VisitorName,
                    vt.TransferDate,
                    vt.Notes
                })
                .ToListAsync();

            return Ok(assignedVisitors);
        }



        [HttpPost("assign")]
        public async Task<IActionResult> AssignVisitors([FromBody] JsonElement request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                List<int> visitorIds;
                int employeeId;
                DateTime? transferDate = null;
                string notes = null;
                string userName = null;

                if (request.TryGetProperty("visitorIds", out var visitorIdsElement))
                {
                    // Frontend format
                    visitorIds = JsonSerializer.Deserialize<List<int>>(visitorIdsElement.GetRawText());
                }
                else if (request.TryGetProperty("VisitorId", out var visitorIdElement))
                {
                    // Postman format
                    visitorIds = new List<int> { visitorIdElement.GetInt32() };
                }
                else
                {
                    return BadRequest("Either visitorIds array or VisitorId is required.");
                }

                // Get employeeId from either format
                if (request.TryGetProperty("employeeId", out var employeeIdElement))
                {
                    employeeId = employeeIdElement.GetInt32();
                }
                else if (request.TryGetProperty("EmployeeId", out var employeeIdElement2))
                {
                    employeeId = employeeIdElement2.GetInt32();
                }
                else
                {
                    return BadRequest("employeeId is required.");
                }

                // Handle optional fields (both naming conventions)
                if (request.TryGetProperty("transferDate", out var transferDateElement) ||
                    request.TryGetProperty("TransferDate", out transferDateElement))
                {
                    if (transferDateElement.ValueKind != JsonValueKind.Null)
                    {
                        transferDate = transferDateElement.GetDateTime();
                    }
                }

                if (request.TryGetProperty("notes", out var notesElement) ||
                    request.TryGetProperty("Notes", out notesElement))
                {
                    notes = notesElement.ValueKind == JsonValueKind.Null ? null : notesElement.GetString();
                }

                if (request.TryGetProperty("userName", out var userNameElement) ||
                    request.TryGetProperty("UserName", out userNameElement))
                {
                    userName = userNameElement.ValueKind == JsonValueKind.Null ? null : userNameElement.GetString();
                }

                // Rest of your existing validation and processing...
                var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == employeeId);
                if (!employeeExists) return NotFound($"Employee with ID {employeeId} not found.");

                foreach (var visitorId in visitorIds)
                {
                    var visitorExists = await _context.Visitors.AnyAsync(v => v.VisitorId == visitorId);
                    if (!visitorExists) return NotFound($"Visitor with ID {visitorId} not found.");

                    var assignmentExists = await _context.visitorTransfer_Junctions
                        .AnyAsync(ve => ve.VisitorId == visitorId && ve.EmployeeId == employeeId);
                    if (assignmentExists) return Conflict($"Visitor {visitorId} is already assigned to employee {employeeId}.");

                    await _context.visitorTransfer_Junctions.AddAsync(new VisitorTransfer_Junction
                    {
                        VisitorId = visitorId,
                        EmployeeId = employeeId,
                        //CreatedDate = null,
                        TransferDate = transferDate ?? DateTime.Now,
                        Notes = notes,
                        UserName = userName
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = $"{visitorIds.Count} visitors assigned successfully to employee {employeeId}."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An error occurred while assigning visitors.",
                    error = ex.Message
                });
            }
        }
        [HttpDelete("DeleteVisitorTransfer/{id}")]
        public async Task<IActionResult> DeleteVisitorTransfer(int id)
        {
            var visitorTransfer = await _context.visitorTransfer_Junctions.FindAsync(id);
            if (visitorTransfer == null)
                return NotFound();

            _context.visitorTransfer_Junctions.Remove(visitorTransfer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // VisitorAssignmentController.cs

        [HttpGet("GetVisitorAssignments/{visitorId}")]
        public async Task<IActionResult> GetVisitorAssignments(int visitorId)
        {
            var history = await _context.visitorTransfer_Junctions
                .Where(x => x.VisitorId == visitorId)
                .Include(x => x.Employee)
                .OrderByDescending(x => x.TransferDate)
                .ToListAsync();

            var result = history.Select(h => new
            {
                h.TransferDate,
                h.Notes,
                h.UserName,
                EmployeeName = h.Employee.EmployeeName
            });

            return Ok(result);
        }

    }
}