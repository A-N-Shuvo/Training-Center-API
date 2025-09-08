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


        private readonly IRepository<Slot> _slotrepository;
        private readonly ILogger<SlotController> _logger;

        public SlotController(IRepository<Slot> slotrepository)
        {
            _slotrepository = slotrepository;
        }


        // get: api/slot
        [HttpGet("GetSlots")]
        public async Task<ActionResult<IEnumerable<Slot>>> getallslots()
        {
            var slots = await _slotrepository.GetAllAsync();
            return Ok(slots);
        }

        // get: api/slot/5
        [HttpGet("GetSlot/{id}")]
        public async Task<ActionResult<Slot>> getslot(int id)
        {
            var slot = await _slotrepository.GetByIdAsync(id);

            if (slot == null)
            {
                return NotFound();
            }

            return Ok(slot);
        }


        [HttpPost("InsertSlot")]
        public async Task<ActionResult<Slot>> createslot(SlotDto slotdto)
        {
            var slot = new Slot
            {
                TimeSlotType = slotdto.TimeSlotType,
                StartTime = TimeOnly.Parse(slotdto.StartTime),
                EndTime = TimeOnly.Parse(slotdto.EndTime),
                IsActive = slotdto.IsActive
            };

            await _slotrepository.AddAsync(slot);
            return CreatedAtAction("GetSlot", new { id = slot.SlotID }, slot);
        }


        // update your updateslot endpoint
        [HttpPut("UpdateSlot/{id}")]
        public async Task<IActionResult> updateslot(int id, [FromBody] SlotDto slotdto)
        {
            if (id != slotdto.SlotID)
            {
                return BadRequest("id mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // parse times
                if (!TimeOnly.TryParse(slotdto.StartTime, out var starttime) ||
                    !TimeOnly.TryParse(slotdto.EndTime, out var endtime))
                {
                    return BadRequest("invalid time format. use hh:mm");
                }

                var slot = new Slot
                {
                    SlotID = slotdto.SlotID,
                    TimeSlotType = slotdto.TimeSlotType,
                    StartTimeString = slotdto.StartTime,
                    EndTimeString = slotdto.EndTime,
                    IsActive = slotdto.IsActive
                };

                await _slotrepository.UpdateAsync(slot);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "an error occurred while updating the slot");
            }
        }



        [HttpDelete("DeleteSlot/{id}")]
        public async Task<IActionResult> deleteslot(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("invalid slot id");
                }

                var slot = await _slotrepository.GetByIdAsync(id);
                if (slot == null)
                {
                    return NotFound();
                }

                await _slotrepository.DeleteAsync(slot);
                return NoContent();
            }
            catch (Exception ex)
            {
                // log the exception (using your preferred logging method)
                _logger.LogError(ex, "error deleting slot with id {slotid}", id);
                return StatusCode(500, "an error occurred while deleting the slot");
            }
        }
    }
}
