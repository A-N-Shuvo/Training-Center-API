using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class ClassRoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClassRoomController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetAllClassRooms")]
        public async Task<ActionResult<IEnumerable<object>>> GetClassRooms()
        {
            return await _context.ClassRooms
                .Include(c => c.ClassRoomCourse_Junction_Tables)
                    .ThenInclude(j => j.Course)
                .Select(c => new
                {
                    c.ClassRoomId,
                    c.RoomName,
                    c.SeatCapacity,
                    c.Location,
                    c.IsActive,
                    c.HasProjector,
                    c.HasAirConditioning,
                    c.HasWhiteboard,
                    c.HasInternetAccess,
                    AssignedCourses = c.ClassRoomCourse_Junction_Tables.Select(j => new
                    {
                        j.CourseId,
                        CourseName = j.Course.CourseName
                    })
                })
                .ToListAsync();
        }

        // GET: api/ClassRoom/GetClassRoom/5
        [HttpGet("GetClassRoom/{id}")]
        public async Task<ActionResult<object>> GetClassRoom(int id)
        {
            var classRoom = await _context.ClassRooms
                .Include(c => c.ClassRoomCourse_Junction_Tables)
                    .ThenInclude(j => j.Course)
                .FirstOrDefaultAsync(c => c.ClassRoomId == id);

            if (classRoom == null)
            {
                return NotFound();
            }

            return new
            {
                classRoom.ClassRoomId,
                classRoom.RoomName,
                classRoom.SeatCapacity,
                classRoom.Location,
                classRoom.HasProjector,
                classRoom.HasAirConditioning,
                classRoom.HasWhiteboard,
                classRoom.HasSoundSystem,
                classRoom.HasInternetAccess,
                classRoom.IsActive,
                classRoom.Remarks,
                classRoom.AdditionalFacilities,
                AssignedCourses = classRoom.ClassRoomCourse_Junction_Tables.Select(j => new
                {
                    j.CourseId,
                    CourseName = j.Course.CourseName,
                    j.IsAvailable
                })
            };
        }


        [HttpPost("InsertClassRoom")]
        public async Task<ActionResult<ClassRoom>> InsertClassRoom([FromBody] ClassRoom classRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create the classroom without the junction tables first
                var newClassRoom = new ClassRoom
                {
                    RoomName = classRoom.RoomName,
                    SeatCapacity = classRoom.SeatCapacity,
                    Location = classRoom.Location,
                    HasProjector = classRoom.HasProjector,
                    HasAirConditioning = classRoom.HasAirConditioning,
                    HasWhiteboard = classRoom.HasWhiteboard,
                    HasSoundSystem = classRoom.HasSoundSystem,
                    HasInternetAccess = classRoom.HasInternetAccess,
                    IsActive = classRoom.IsActive,
                    Remarks = classRoom.Remarks,
                    AdditionalFacilities = classRoom.AdditionalFacilities
                };

                _context.ClassRooms.Add(newClassRoom);
                await _context.SaveChangesAsync();

                // Process junction table if provided
                if (classRoom.ClassRoomCourse_Junction_Tables != null &&
                    classRoom.ClassRoomCourse_Junction_Tables.Any())
                {
                    foreach (var junction in classRoom.ClassRoomCourse_Junction_Tables)
                    {
                        // Verify course exists
                        if (!await _context.Courses.AnyAsync(c => c.CourseId == junction.CourseId))
                        {
                            await transaction.RollbackAsync();
                            return BadRequest($"Course with ID {junction.CourseId} does not exist");
                        }

                        // Create new junction entry
                        var newJunction = new ClassRoomCourse_Junction_Table
                        {
                            ClassRoomId = newClassRoom.ClassRoomId,
                            CourseId = junction.CourseId,
                            IsAvailable = junction.IsAvailable
                        };

                        _context.ClassRoomCourse_Junction_Tables.Add(newJunction);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // Return the created classroom with junction data
                var result = await _context.ClassRooms
                    .Include(c => c.ClassRoomCourse_Junction_Tables)
                    .ThenInclude(j => j.Course)
                    .FirstOrDefaultAsync(c => c.ClassRoomId == newClassRoom.ClassRoomId);

                return CreatedAtAction(nameof(GetClassRoom), new { id = result.ClassRoomId }, result);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("UpdateClassRoom/{id}")]
        public async Task<IActionResult> UpdateClassRoom(int id, [FromBody] ClassRoom classRoom)
        {
            if (id != classRoom.ClassRoomId)
            {
                return BadRequest("Classroom ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Update the main classroom properties
                var existingClassRoom = await _context.ClassRooms
                    .Include(c => c.ClassRoomCourse_Junction_Tables)
                    .FirstOrDefaultAsync(c => c.ClassRoomId == id);

                if (existingClassRoom == null)
                {
                    return NotFound($"Classroom with ID {id} not found");
                }

                // Update properties
                existingClassRoom.RoomName = classRoom.RoomName;
                existingClassRoom.SeatCapacity = classRoom.SeatCapacity;
                existingClassRoom.Location = classRoom.Location;
                existingClassRoom.HasProjector = classRoom.HasProjector;
                existingClassRoom.HasAirConditioning = classRoom.HasAirConditioning;
                existingClassRoom.HasWhiteboard = classRoom.HasWhiteboard;
                existingClassRoom.HasSoundSystem = classRoom.HasSoundSystem;
                existingClassRoom.HasInternetAccess = classRoom.HasInternetAccess;
                existingClassRoom.IsActive = classRoom.IsActive;
                existingClassRoom.Remarks = classRoom.Remarks;
                existingClassRoom.AdditionalFacilities = classRoom.AdditionalFacilities;

                // Handle junction table updates
                if (classRoom.ClassRoomCourse_Junction_Tables != null)
                {
                    // Remove existing junctions not in the new list
                    var existingJunctions = existingClassRoom.ClassRoomCourse_Junction_Tables.ToList();
                    var newJunctionCourseIds = classRoom.ClassRoomCourse_Junction_Tables
                        .Select(j => j.CourseId)
                        .ToList();

                    foreach (var existingJunction in existingJunctions)
                    {
                        if (!newJunctionCourseIds.Contains(existingJunction.CourseId))
                        {
                            _context.ClassRoomCourse_Junction_Tables.Remove(existingJunction);
                        }
                    }

                    // Add or update junctions
                    foreach (var junction in classRoom.ClassRoomCourse_Junction_Tables)
                    {
                        var existingJunction = existingClassRoom.ClassRoomCourse_Junction_Tables
                            .FirstOrDefault(j => j.CourseId == junction.CourseId);

                        if (existingJunction == null)
                        {
                            // Verify course exists
                            if (!await _context.Courses.AnyAsync(c => c.CourseId == junction.CourseId))
                            {
                                await transaction.RollbackAsync();
                                return BadRequest($"Course with ID {junction.CourseId} does not exist");
                            }

                            // Add new junction
                            var newJunction = new ClassRoomCourse_Junction_Table
                            {
                                ClassRoomId = id,
                                CourseId = junction.CourseId,
                                IsAvailable = junction.IsAvailable
                            };
                            _context.ClassRoomCourse_Junction_Tables.Add(newJunction);
                        }
                        else
                        {
                            // Update existing junction
                            existingJunction.IsAvailable = junction.IsAvailable;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!ClassRoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteClassRoom/{id}")]
        public async Task<IActionResult> DeleteClassRoom(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var classRoom = await _context.ClassRooms
                    .Include(c => c.ClassRoomCourse_Junction_Tables)
                    .Include(c => c.Batches)
                    .FirstOrDefaultAsync(c => c.ClassRoomId == id);

                if (classRoom == null)
                {
                    return NotFound();
                }

                // Check if classroom is used in any batches
                if (classRoom.Batches != null && classRoom.Batches.Any())
                {
                    return BadRequest("Cannot delete classroom as it is assigned to one or more batches");
                }

                // Remove all junction table entries
                if (classRoom.ClassRoomCourse_Junction_Tables != null)
                {
                    _context.ClassRoomCourse_Junction_Tables.RemoveRange(classRoom.ClassRoomCourse_Junction_Tables);
                }

                _context.ClassRooms.Remove(classRoom);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool ClassRoomExists(int id)
        {
            return _context.ClassRooms.Any(e => e.ClassRoomId == id);
        }
    }
}