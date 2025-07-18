using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ApplicationDbContext _context; // Add this field

        public EmployeeController(IRepository<Employee> employeeRepository, ApplicationDbContext context)
        {
            _employeeRepository = employeeRepository;
            _context = context;
        }

        

        [HttpGet("GetEmployees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("GetEmployee/{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }


        // PUT: api/Employees/5
        [HttpPut, Route("UpdateEmployee/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> PutEmployee(int id, [FromForm] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }

            try
            {
                // Get existing employee data
                var existingEmployee = await _employeeRepository.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    return NotFound();
                }

                // ModelState validation check
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid data provided",
                        Errors = errors
                    });
                }

                // Get base directory path
                var basePath = Directory.GetCurrentDirectory();

                // Process image file if provided
                if (employee.ImageFile != null && employee.ImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingEmployee.ImagePath))
                    {
                        var oldImagePath = Path.Combine(basePath, existingEmployee.ImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    var imageFolder = Path.Combine(basePath, "Images", "Employees");
                    Directory.CreateDirectory(imageFolder);

                    var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(employee.ImageFile.FileName);
                    var imagePath = Path.Combine(imageFolder, imageFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await employee.ImageFile.CopyToAsync(stream);
                    }

                    employee.ImagePath = Path.Combine("Images", "Employees", imageFileName);
                }
                else
                {
                    // Keep the existing image path if no new file is provided
                    employee.ImagePath = existingEmployee.ImagePath;
                }

                // Process document file if provided
                if (employee.DocumentFile != null && employee.DocumentFile.Length > 0)
                {
                    // Delete old document if exists
                    if (!string.IsNullOrEmpty(existingEmployee.DocumentPath))
                    {
                        var oldDocPath = Path.Combine(basePath, existingEmployee.DocumentPath);
                        if (System.IO.File.Exists(oldDocPath))
                        {
                            System.IO.File.Delete(oldDocPath);
                        }
                    }

                    // Save new document
                    var docFolder = Path.Combine(basePath, "Documents", "Employees");
                    Directory.CreateDirectory(docFolder);

                    var docFileName = Guid.NewGuid().ToString() + Path.GetExtension(employee.DocumentFile.FileName);
                    var docPath = Path.Combine(docFolder, docFileName);

                    using (var stream = new FileStream(docPath, FileMode.Create))
                    {
                        await employee.DocumentFile.CopyToAsync(stream);
                    }

                    employee.DocumentPath = Path.Combine("Documents", "Employees", docFileName);
                }
                else
                {
                    // Keep the existing document path if no new file is provided
                    employee.DocumentPath = existingEmployee.DocumentPath;
                }

                employee.EmployeeIDNo = existingEmployee.EmployeeIDNo;


                // Update other properties
                //existingEmployee.EmployeeIDNo = employee.EmployeeIDNo;
                existingEmployee.EmployeeName = employee.EmployeeName;
                existingEmployee.DesignationId = employee.DesignationId;
                existingEmployee.DepartmentId = employee.DepartmentId;
                existingEmployee.ContactNo = employee.ContactNo;
                existingEmployee.DOB = employee.DOB;
                existingEmployee.JoiningDate = employee.JoiningDate;
                // Only update EndDate if it's provided
                if (employee.EndDate.HasValue)
                {
                    existingEmployee.EndDate = employee.EndDate;
                }
                existingEmployee.EmailAddress = employee.EmailAddress;
                existingEmployee.PermanentAddress = employee.PermanentAddress;
                existingEmployee.PresentAddress = employee.PresentAddress;
                existingEmployee.FathersName = employee.FathersName;
                existingEmployee.MothersName = employee.MothersName;
                existingEmployee.BirthOrNIDNo = employee.BirthOrNIDNo;
                existingEmployee.IsAvailable = employee.IsAvailable;
                existingEmployee.IsWillingToSell = employee.IsWillingToSell;
                existingEmployee.ImagePath = employee.ImagePath;
                existingEmployee.DocumentPath = employee.DocumentPath;
                existingEmployee.Remarks = employee.Remarks;


                await _employeeRepository.UpdateAsync(existingEmployee);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error",
                    Error = ex.Message
                });
            }
        }

        

        [HttpPost("InsertEmployee")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Employee>> PostEmployee([FromForm] Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid data provided",
                        Errors = errors
                    });
                }

                // First add the employee to generate the auto-incremented EmployeeId
                await _employeeRepository.AddAsync(employee);

                // Now update the EmployeeIDNo based on the generated EmployeeId
                employee.EmployeeIDNo = $"Emp-{employee.EmployeeId:D5}";
                await _employeeRepository.UpdateAsync(employee);

                var basePath = Directory.GetCurrentDirectory();

                if (employee.ImageFile != null && employee.ImageFile.Length > 0)
                {
                    var imageFolder = Path.Combine(basePath, "Images", "Employees");
                    Directory.CreateDirectory(imageFolder);

                    var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(employee.ImageFile.FileName);
                    var imagePath = Path.Combine(imageFolder, imageFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await employee.ImageFile.CopyToAsync(stream);
                    }

                    employee.ImagePath = Path.Combine("Images", "Employees", imageFileName);
                    await _employeeRepository.UpdateAsync(employee);
                }

                if (employee.DocumentFile != null && employee.DocumentFile.Length > 0)
                {
                    var docFolder = Path.Combine(basePath, "Documents", "Employees");
                    Directory.CreateDirectory(docFolder);

                    var docFileName = Guid.NewGuid().ToString() + Path.GetExtension(employee.DocumentFile.FileName);
                    var docPath = Path.Combine(docFolder, docFileName);

                    using (var stream = new FileStream(docPath, FileMode.Create))
                    {
                        await employee.DocumentFile.CopyToAsync(stream);
                    }

                    employee.DocumentPath = Path.Combine("Documents", "Employees", docFileName);
                    await _employeeRepository.UpdateAsync(employee);
                }

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error",
                    Error = ex.Message
                });
            }
        }

        //[HttpDelete("deleteemployee/{id}")]
        //public async task<ActionResult> deleteemployee(int id)
        //{
        //    try
        //    {
        //        var employee = await _employeerepository.getbyidasync(id);
        //        if (employee == null)
        //        {
        //            return notfound($"employee with id {id} not found.");
        //        }

        //        await _employeerepository.deleteasync(employee);
        //        return nocontent();
        //    }
        //    catch (exception ex)
        //    {
        //        // log the exception (you should have proper logging configured)
        //        return statuscode(500, $"internal server error: {ex.message}");
        //    }
        //}


        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                    return NotFound();

                await _employeeRepository.DeleteAsync(employee);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the employee.");
            }
        }


        [HttpPut("MarkAsUnavailable/{id}")]  // PUT is more appropriate for updates
        public async Task<IActionResult> MarkAsUnavailable(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }

                // Soft delete by setting IsAvailable = false
                employee.IsAvailable = false;
                await _employeeRepository.UpdateAsync(employee);  // Save changes

                return Ok(new { message = "Employee marked as unavailable successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Employees/Available
        [HttpGet("Available")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAvailableEmployees()
        {
            // Note: You might need to extend your repository for this specific query
            var allEmployees = await _employeeRepository.GetAllAsync();
            var availableEmployees = allEmployees.Where(e => e.IsAvailable);
            return Ok(availableEmployees);
        }

        // GET: api/Employees/WillingToSell
        [HttpGet("WillingToSell")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetWillingToSellEmployees()
        {
            // Note: You might need to extend your repository for this specific query
            var allEmployees = await _employeeRepository.GetAllAsync();
            var willingEmployees = allEmployees.Where(e => e.IsWillingToSell);
            return Ok(willingEmployees);
        }
    }
}