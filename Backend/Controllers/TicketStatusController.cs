using Backend.Data;
using Backend.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketStatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketStatusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketStatus>>> GetStatuses()
        {
            try
            {
                var statuses = await _context.TicketStatuses.ToListAsync();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving statuses", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketStatus>> GetStatus(int id)
        {
            try
            {
                var status = await _context.TicketStatuses
                    .Include(s => s.Tickets)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (status == null)
                    return NotFound(new { message = "Status not found" });

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving status", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TicketStatus>> CreateStatus([FromBody] TicketStatus status)
        {
            try
            {
                if (status == null)
                    return BadRequest(new { message = "Status cannot be null" });

                if (string.IsNullOrWhiteSpace(status.StatusName))
                    return BadRequest(new { message = "StatusName is required" });

                _context.TicketStatuses.Add(status);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetStatus", new { id = status.Id }, status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating status", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TicketStatus status)
        {
            try
            {
                if (id != status.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var existingStatus = await _context.TicketStatuses.FindAsync(id);
                if (existingStatus == null)
                    return NotFound(new { message = "Status not found" });

                existingStatus.StatusName = status.StatusName;

                _context.Entry(existingStatus).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating status", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                var status = await _context.TicketStatuses.FindAsync(id);
                if (status == null)
                    return NotFound(new { message = "Status not found" });

                _context.TicketStatuses.Remove(status);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting status", error = ex.Message });
            }
        }
    }
}
