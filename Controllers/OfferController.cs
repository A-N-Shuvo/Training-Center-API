using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingCenter_Api.DAL.Interfaces;
using TrainingCenter_Api.Data;
using TrainingCenter_Api.Models;

namespace TrainingCenter_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]

    public class OfferController : ControllerBase
    {
        private readonly IRepository<Offer> _offerRepository;
        private readonly ApplicationDbContext _context;

        public OfferController(IRepository<Offer> offerRepository, ApplicationDbContext context)
        {
            _offerRepository = offerRepository;
            _context = context;
        }

        [HttpGet("GetActiveOffers")]
        public async Task<ActionResult<IEnumerable<Offer>>> GetActiveOffers()
        {
            // Get current date
            var currentDate = DateTime.Now;

            // Return offers where:
            // - StartDate <= current date
            // - EndDate >= current date or null (no end date)
            // - IsActive == true
            return await _context.Offers
                .Where(o => o.IsActive )
                .ToListAsync();
        }


        // GET: api/Offer
        [HttpGet("GetOffers")]
        public async Task<ActionResult<IEnumerable<Offer>>> GetOffers()
        {
            var offers = await _offerRepository.GetAllAsync();
            return Ok(offers);
        }

        // GET: api/Offer/5
        [HttpGet("GetOffer/{id}")]
        public async Task<ActionResult<Offer>> GetOffer(int id)
        {
            var offer = await _offerRepository.GetByIdAsync(id);
            if (offer == null)
                return NotFound();

            return Ok(offer);
        }

        // POST: api/Offer
        [HttpPost("InsertOffer")]
        public async Task<ActionResult<Offer>> PostOffer(Offer offer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _offerRepository.AddAsync(offer);
            return CreatedAtAction(nameof(GetOffer), new { id = offer.OfferId }, offer);
        }



        // PUT: api/Offer/5

        [HttpPut, Route("UpdateOffer/{id}")]
        public async Task<IActionResult> PutOffer(int id, Offer offer)
        {
            if (id != offer.OfferId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _offerRepository.UpdateAsync(offer);
            }
            catch
            {
                if (!await _offerRepository.ExistsAsync(id))
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


        // DELETE: api/Offer/5
        [HttpDelete("DeleteOffer/{id}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var offer = await _offerRepository.GetByIdAsync(id);
            if (offer == null)
                return NotFound();

            await _offerRepository.DeleteAsync(offer);
            return NoContent();

        }
    }
}
