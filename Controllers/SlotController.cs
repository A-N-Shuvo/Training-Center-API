using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;
using TrainingCenter_Api.Models.DTOs;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SlotController : ControllerBase
    {

        //==========For Angular========
        //        private readonly IRepository<Slot> _slotRepository;
        //        private readonly ApplicationDbContext _context;

        //        public SlotController(IRepository<Slot> slotRepository, ApplicationDbContext context)
        //        {
        //            _slotRepository = slotRepository;
        //            _context = context;
        //        }

        //        // GET: api/Slot
        //        [HttpGet("GetSlots")]
        //        public async Task<ActionResult<IEnumerable<Slot>>> GetAllSlots()
        //        {
        //            var slots = await _slotRepository.GetAllAsync();
        //            return Ok(slots);
        //        }


        //        // GET: api/Slot/5
        //        [HttpGet("GetSlot/{id}")]
        //        public async Task<ActionResult<Slot>> GetSlot(int id)
        //        {
        //            var slot = await _slotRepository.GetByIdAsync(id);

        //            if (slot == null)
        //            {
        //                return NotFound();
        //            }

        //            return Ok(slot);
        //        }

        //        // POST: api/Slot/InsertSlot
        //        [HttpPost("InsertSlot")]
        //        public async Task<ActionResult<Slot>> CreateSlot([FromBody] Slot slot)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(ModelState);

        //            // Validate time slot
        //            if (slot.StartTime >= slot.EndTime)
        //            {
        //                return BadRequest("End time must be after start time");
        //            }

        //            // শুধুমাত্র ডুপ্লিকেট চেক (একই টাইপ + একই সময়)
        //            bool duplicateSlot = await _context.Slots.AnyAsync(s =>
        //                s.TimeSlotType == slot.TimeSlotType &&
        //                s.StartTime == slot.StartTime &&
        //                s.EndTime == slot.EndTime);

        //            if (duplicateSlot)
        //            {
        //                return Conflict("A slot with the same type and timing already exists");
        //            }

        //            try
        //            {
        //                await _slotRepository.AddAsync(slot);
        //                return CreatedAtAction(nameof(GetSlot), new { id = slot.SlotID }, slot);
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, "An error occurred while creating the slot");
        //            }
        //        }

        //        // PUT: api/Slot/UpdateSlot/5
        //        [HttpPut("UpdateSlot/{id}")]
        //        public async Task<IActionResult> UpdateSlot(int id, [FromBody] Slot slot)
        //        {
        //            if (id != slot.SlotID)
        //                return BadRequest("ID mismatch");

        //            if (!ModelState.IsValid)
        //                return BadRequest(ModelState);

        //            // Validate time slot
        //            if (slot.StartTime >= slot.EndTime)
        //            {
        //                return BadRequest("End time must be after start time");
        //            }

        //            // শুধুমাত্র ডুপ্লিকেট চেক (একই টাইপ + একই সময়)
        //            bool duplicateSlot = await _context.Slots.AnyAsync(s =>
        //                s.SlotID != id && // নিজেকে বাদ দিতে
        //                s.TimeSlotType == slot.TimeSlotType &&
        //                s.StartTime == slot.StartTime &&
        //                s.EndTime == slot.EndTime);

        //            if (duplicateSlot)
        //            {
        //                return Conflict("A slot with the same type and timing already exists");
        //            }

        //            try
        //            {
        //                await _slotRepository.UpdateAsync(slot);
        //                return NoContent();
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, "An error occurred while updating the slot");
        //            }
        //        }

        //        // DELETE: api/Slot/5
        //        [HttpDelete("DeleteSlot/{id}")]
        //        public async Task<IActionResult> DeleteSlot(int id)
        //        {
        //            var slot = await _slotRepository.GetByIdAsync(id);
        //            if (slot == null)
        //            {
        //                return NotFound();
        //            }

        //            await _slotRepository.DeleteAsync(slot);
        //            return NoContent();
        //        }
        //    }
        //}


        //===========For React===========
        private readonly IRepository<Slot> _slotRepository;
        private readonly ILogger<SlotController> _logger;

        public SlotController(IRepository<Slot> slotRepository)
        {
            _slotRepository = slotRepository;
        }


        // GET: api/Slot
        [HttpGet("GetSlots")]
        public async Task<ActionResult<IEnumerable<Slot>>> GetAllSlots()
        {
            var slots = await _slotRepository.GetAllAsync();
            return Ok(slots);
        }

        // GET: api/Slot/5
        [HttpGet("GetSlot/{id}")]
        public async Task<ActionResult<Slot>> GetSlot(int id)
        {
            var slot = await _slotRepository.GetByIdAsync(id);

            if (slot == null)
            {
                return NotFound();
            }

            return Ok(slot);
        }


        [HttpPost("InsertSlot")]
        public async Task<ActionResult<Slot>> CreateSlot(SlotDto slotDto)
        {
            var slot = new Slot
            {
                TimeSlotType = slotDto.TimeSlotType,
                StartTime = TimeOnly.Parse(slotDto.StartTime),
                EndTime = TimeOnly.Parse(slotDto.EndTime),
                IsActive = slotDto.IsActive
            };

            await _slotRepository.AddAsync(slot);
            return CreatedAtAction("GetSlot", new { id = slot.SlotID }, slot);
        }


        // Update your UpdateSlot endpoint
        [HttpPut("UpdateSlot/{id}")]
        public async Task<IActionResult> UpdateSlot(int id, [FromBody] SlotDto slotDto)
        {
            if (id != slotDto.SlotID)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Parse times
                if (!TimeOnly.TryParse(slotDto.StartTime, out var startTime) ||
                    !TimeOnly.TryParse(slotDto.EndTime, out var endTime))
                {
                    return BadRequest("Invalid time format. Use HH:mm");
                }

                var slot = new Slot
                {
                    SlotID = slotDto.SlotID,
                    TimeSlotType = slotDto.TimeSlotType,
                    StartTimeString = slotDto.StartTime,
                    EndTimeString = slotDto.EndTime,
                    IsActive = slotDto.IsActive
                };

                await _slotRepository.UpdateAsync(slot);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the slot");
            }
        }



        [HttpDelete("DeleteSlot/{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid slot ID");
                }

                var slot = await _slotRepository.GetByIdAsync(id);
                if (slot == null)
                {
                    return NotFound();
                }

                await _slotRepository.DeleteAsync(slot);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception (using your preferred logging method)
                _logger.LogError(ex, "Error deleting slot with ID {SlotId}", id);
                return StatusCode(500, "An error occurred while deleting the slot");
            }
        }
    }
}
