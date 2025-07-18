using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRepository<Registration> _registrationRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public RegistrationController(IRepository<Registration> registrationRepository, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _registrationRepository = registrationRepository;
            _environment = environment;
            _context = context;
        }


        [HttpGet("GetByVisitor/{visitorId}")]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrationsByVisitor(int visitorId)
        {
            var registrations = await _context.Registrations
                .Include(r => r.Visitor)
                .Include(r => r.Course)
                .Include(r => r.CourseCombo)
                .Where(r => r.VisitorId == visitorId)
                .ToListAsync();

            //if (!registrations.Any())
            //{
            //    return NotFound("No registrations found for this visitor");
            //}

            return Ok(registrations);
        }





        [HttpGet("GenerateRegistrationNo")]
        public IActionResult GenerateRegistrationNo()
        {
            try
            {
                // Get the next available RegistrationId
                var nextId = _context.Registrations.Any()
                    ? _context.Registrations.Max(r => r.RegistrationId) + 1
                    : 1;

                // Format with leading zeros
                var newRegistrationNo = $"Reg-{nextId:D6}";

                return Ok(new { registrationNo = newRegistrationNo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // GET: api/Registrations
        [HttpGet("GetRegistrations")]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
        {
            var registrations = await _registrationRepository.GetAllAsync();
            return Ok(registrations);
        }

        // GET: api/Registrations/5
        [HttpGet("GetRegistration/{id}")]
        public async Task<ActionResult<Registration>> GetRegistration(int id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);

            if (registration == null)
            {
                return NotFound();
            }

            return registration;
        }




        [HttpPut, Route("UpdateRegistration/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateRegistration(int id, [FromForm] Registration registration)
        {
            if (id != registration.RegistrationId)
            {
                return BadRequest();
            }

            try
            {
                var existingRegistration = await _registrationRepository.GetByIdAsync(id);
                if (existingRegistration == null)
                {
                    return NotFound();
                }

                // Always preserve the registration number
                registration.RegistraionNo = existingRegistration.RegistraionNo;


                // Get base directory path
                var basePath = Directory.GetCurrentDirectory();

                // Process image file if provided
                if (registration.ImageFile != null && registration.ImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingRegistration.ImagePath))
                    {
                        var oldImagePath = Path.Combine(basePath, existingRegistration.ImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save new image
                    var imageFolder = Path.Combine(basePath, "Images", "Registration");
                    Directory.CreateDirectory(imageFolder);

                    var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(registration.ImageFile.FileName);
                    var imagePath = Path.Combine(imageFolder, imageFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await registration.ImageFile.CopyToAsync(stream);
                    }

                    existingRegistration.ImagePath = Path.Combine("Images", "Registration", imageFileName);
                }

                // Process document file if provided
                if (registration.DocumentFile != null && registration.DocumentFile.Length > 0)
                {
                    // Delete old document if exists
                    if (!string.IsNullOrEmpty(existingRegistration.DocumentPath))
                    {
                        var oldDocPath = Path.Combine(basePath, existingRegistration.DocumentPath);
                        if (System.IO.File.Exists(oldDocPath))
                        {
                            System.IO.File.Delete(oldDocPath);
                        }
                    }

                    // Save new document
                    var docFolder = Path.Combine(basePath, "Documents", "Registration");
                    Directory.CreateDirectory(docFolder);

                    var docFileName = Guid.NewGuid().ToString() + Path.GetExtension(registration.DocumentFile.FileName);
                    var docPath = Path.Combine(docFolder, docFileName);

                    using (var stream = new FileStream(docPath, FileMode.Create))
                    {
                        await registration.DocumentFile.CopyToAsync(stream);
                    }

                    existingRegistration.DocumentPath = Path.Combine("Documents", "Registration", docFileName);
                }

                // Update all other properties
                existingRegistration.VisitorId = registration.VisitorId;
                existingRegistration.TraineeName = registration.TraineeName;
                existingRegistration.RegistrationDate = registration.RegistrationDate;
                existingRegistration.CourseId = registration.CourseId; // Updated for direct CourseId
                existingRegistration.CourseComboId = registration.CourseComboId;
                existingRegistration.Gender = registration.Gender;
                existingRegistration.Nationality = registration.Nationality;
                existingRegistration.Religion = registration.Religion;
                existingRegistration.DateOfBirth = registration.DateOfBirth;
                existingRegistration.OriginatDateofBirth = registration.OriginatDateofBirth;
                existingRegistration.MaritalStatus = registration.MaritalStatus;
                existingRegistration.FatherName = registration.FatherName;
                existingRegistration.MotherName = registration.MotherName;
                existingRegistration.ContactNo = registration.ContactNo;
                existingRegistration.EmergencyContactNo = registration.EmergencyContactNo;
                existingRegistration.EmailAddress = registration.EmailAddress;
                existingRegistration.BloodGroup = registration.BloodGroup;
                existingRegistration.BirthOrNIDNo = registration.BirthOrNIDNo;
                existingRegistration.PresentAddress = registration.PresentAddress;
                existingRegistration.PermanentAddress = registration.PermanentAddress;
                existingRegistration.HighestEducation = registration.HighestEducation;
                existingRegistration.InstitutionName = registration.InstitutionName;
                existingRegistration.Reference = registration.Reference;
                existingRegistration.Remarks = registration.Remarks;


                await _registrationRepository.UpdateAsync(existingRegistration);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        private void DeleteFile(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }





        [HttpPost("InsertRegistration")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Registration>> CreateRegistration([FromForm] Registration registration)
        {
            try
            {
                // No need to process SelectedCourseIds anymore as we're using direct CourseId foreign key

                // Get the base directory path
                var basePath = Directory.GetCurrentDirectory();

                // Process image file
                if (registration.ImageFile != null && registration.ImageFile.Length > 0)
                {
                    var imageFolder = Path.Combine(basePath, "Images", "Registration");
                    Directory.CreateDirectory(imageFolder);

                    var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(registration.ImageFile.FileName);
                    var imagePath = Path.Combine(imageFolder, imageFileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await registration.ImageFile.CopyToAsync(stream);
                    }

                    registration.ImagePath = Path.Combine("Images", "Registration", imageFileName);
                }

                // Process document file
                if (registration.DocumentFile != null && registration.DocumentFile.Length > 0)
                {
                    var docFolder = Path.Combine(basePath, "Documents", "Registration");
                    Directory.CreateDirectory(docFolder);

                    var docFileName = Guid.NewGuid().ToString() + Path.GetExtension(registration.DocumentFile.FileName);
                    var docPath = Path.Combine(docFolder, docFileName);

                    using (var stream = new FileStream(docPath, FileMode.Create))
                    {
                        await registration.DocumentFile.CopyToAsync(stream);
                    }

                    registration.DocumentPath = Path.Combine("Documents", "Registration", docFileName);
                }

                // Add the registration to get the ID
                await _registrationRepository.AddAsync(registration);

                // Generate registration number
                registration.RegistraionNo = $"Reg-{registration.RegistrationId:D5}";

                // Update the registration with the generated number
                await _registrationRepository.UpdateAsync(registration);

                return CreatedAtAction(nameof(GetRegistration), new { id = registration.RegistrationId }, registration);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpDelete("DeleteRegistration/{id}")]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
            {
                return NotFound();
            }

            await _registrationRepository.DeleteAsync(registration);
            return NoContent();
        }
    }
}