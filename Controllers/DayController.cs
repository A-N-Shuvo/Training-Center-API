using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DayController : ControllerBase
    {

        //        //==================For Angular============

        //        private readonly IRepository<Day> _dayRepository;
        //        private ApplicationDbContext _context;

        //        public DayController(IRepository<Day> dayRepository, ApplicationDbContext context)
        //        {
        //            _dayRepository = dayRepository;
        //            _context = context;
        //        }

        //        // GET: api/Day/GetDays
        //        [HttpGet("GetDays")]
        //        public async Task<ActionResult<IEnumerable<Day>>> GetAllDays()
        //        {
        //            var days = await _dayRepository.GetAllAsync();
        //            return Ok(days);
        //        }

        //        // GET: api/Day/GetDay/5
        //        [HttpGet("GetDay/{id}")]
        //        public async Task<ActionResult<Day>> GetDay(int id)
        //        {
        //            var day = await _dayRepository.GetByIdAsync(id);
        //            return day == null ? NotFound() : Ok(day);
        //        }

        //        // POST: api/Day/InsertDay
        //        [HttpPost("InsertDay")]
        //        public async Task<ActionResult<Day>> CreateDay([FromBody] Day day)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(ModelState);

        //            // Check for duplicate day name
        //            bool dayExists = await _dayRepository.AnyAsync(d => d.DayName == day.DayName);
        //            if (dayExists)
        //            {
        //                return Conflict($"A day with the name '{day.DayName}' already exists.");
        //            }

        //            try
        //            {
        //                await _dayRepository.AddAsync(day);
        //                return CreatedAtAction(nameof(GetDay), new { id = day.DayId }, day);
        //            }
        //            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        //            {
        //                // Handle SQL Server unique constraint violation
        //                return Conflict($"A day with the name '{day.DayName}' already exists.");
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the day.");
        //            }
        //        }

        //        // PUT: api/Day/UpdateDay/5
        //        [HttpPut("UpdateDay/{id}")]
        //        public async Task<IActionResult> UpdateDay(int id, [FromBody] Day day)
        //        {
        //            if (id != day.DayId)
        //            {
        //                return BadRequest("ID mismatch");
        //            }

        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            // Check for duplicate day name excluding the current day
        //            bool dayExists = await _dayRepository.AnyAsync(d => d.DayName == day.DayName && d.DayId != day.DayId);
        //            if (dayExists)
        //            {
        //                return Conflict($"A day with the name '{day.DayName}' already exists.");
        //            }

        //            try
        //            {
        //                await _dayRepository.UpdateAsync(day);
        //                return NoContent();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!await _dayRepository.ExistsAsync(id))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        //            {
        //                return Conflict($"A day with the name '{day.DayName}' already exists.");
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the day.");
        //            }
        //        }

        //        // DELETE: api/Day/DeleteDay/5
        //        [HttpDelete("DeleteDay/{id}")]
        //        public async Task<IActionResult> DeleteDay(int id)
        //        {
        //            try
        //            {
        //                var day = await _dayRepository.GetByIdAsync(id);
        //                if (day == null)
        //                    return NotFound();

        //                await _dayRepository.DeleteAsync(day);
        //                return NoContent();
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the day.");
        //            }
        //        }


        //    }
        //}


        //===============For React==========

        private readonly IRepository<Day> _dayRepository;

        public DayController(IRepository<Day> dayRepository)
        {
            _dayRepository = dayRepository;
        }

        // GET: api/Day/GetDays
        [HttpGet("GetDays")]
        public async Task<ActionResult<ResponseModel<IEnumerable<Day>>>> GetAllDays()
        {
            var days = await _dayRepository.GetAllAsync();
            return Ok(new ResponseModel<IEnumerable<Day>>(true, "Data retrieved", days));
        }

        // GET: api/Day/GetDay/5
        [HttpGet("GetDay/{id}")]
        public async Task<ActionResult<ResponseModel<Day>>> GetDay(int id)
        {
            var day = await _dayRepository.GetByIdAsync(id);
            if (day == null)
                return NotFound(new ResponseModel<Day>(false, "Day not found"));

            return Ok(new ResponseModel<Day>(true, "Data retrieved", day));
        }

        // POST: api/Day/InsertDay
        [HttpPost("InsertDay")]
        public async Task<ActionResult<ResponseModel<Day>>> CreateDay([FromBody] Day day)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel<Day>(false, "Invalid data", null, ModelState));

            await _dayRepository.AddAsync(day);
            return CreatedAtAction(nameof(GetDay),
                new { id = day.DayId },
                new ResponseModel<Day>(true, "Day created", day));
        }

        // PUT: api/Day/UpdateDay/5
        [HttpPut("UpdateDay/{id}")]
        public async Task<ActionResult<ResponseModel<Day>>> UpdateDay(int id, [FromBody] Day day)
        {
            if (id != day.DayId)
                return BadRequest(new ResponseModel<Day>(false, "ID mismatch"));

            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel<Day>(false, "Invalid data", null, ModelState));

            try
            {
                await _dayRepository.UpdateAsync(day);
                var updatedDay = await _dayRepository.GetByIdAsync(id); // Fetch updated data
                return Ok(new ResponseModel<Day>(true, "Day updated", updatedDay));
            }
            catch
            {
                if (!await _dayRepository.ExistsAsync(id))
                    return NotFound(new ResponseModel<Day>(false, "Day not found"));

                throw;
            }
        }

        // DELETE: api/Day/DeleteDay/5
        [HttpDelete("DeleteDay/{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> DeleteDay(int id)
        {
            var day = await _dayRepository.GetByIdAsync(id);
            if (day == null)
                return NotFound(new ResponseModel<bool>(false, "Day not found"));

            await _dayRepository.DeleteAsync(day);
            return Ok(new ResponseModel<bool>(true, "Day deleted", true));
        }
    }

    // Add this class in your models
    public class ResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public ResponseModel(bool isSuccess, string message, T data = default)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public ResponseModel(bool isSuccess, string message, T data, ModelStateDictionary modelState)
            : this(isSuccess, message, data)
        {
            Errors = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );
        }
    }
}
