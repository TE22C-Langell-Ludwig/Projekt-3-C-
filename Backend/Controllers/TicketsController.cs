using Backend.Data;
using Backend.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTickets()
        {
            try
            {
                var tickets = await _context.Tickets
                    .Include(t => t.CreatorUser)
                    .Include(t => t.Status)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.DateCreated,
                        t.CreatorUserId,
                        CreatorUser = new { t.CreatorUser.Id, t.CreatorUser.Username, t.CreatorUser.Role },
                        t.StatusId,
                        Status = new { t.Status.Id, t.Status.StatusName }
                    })
                    .ToListAsync();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tickets", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTicket(int id)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.CreatorUser)
                    .Include(t => t.Status)
                    .Where(t => t.Id == id)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.DateCreated,
                        t.CreatorUserId,
                        CreatorUser = new { t.CreatorUser.Id, t.CreatorUser.Username, t.CreatorUser.Role },
                        t.StatusId,
                        Status = new { t.Status.Id, t.Status.StatusName }
                    })
                    .FirstOrDefaultAsync();

                if (ticket == null)
                    return NotFound(new { message = "Ticket not found" });

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving ticket", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket([FromBody] Ticket ticket)
        {
            try
            {
                if (ticket == null)
                    return BadRequest(new { message = "Ticket cannot be null" });

                if (string.IsNullOrWhiteSpace(ticket.Title))
                    return BadRequest(new { message = "Title is required" });

                if (ticket.CreatorUserId <= 0)
                    return BadRequest(new { message = "Valid CreatorUserId is required" });

                if (ticket.StatusId <= 0)
                    return BadRequest(new { message = "Valid StatusId is required" });

                var userExists = await _context.Users.AnyAsync(u => u.Id == ticket.CreatorUserId);
                if (!userExists)
                    return BadRequest(new { message = "Creator user does not exist" });

                var statusExists = await _context.TicketStatuses.AnyAsync(s => s.Id == ticket.StatusId);
                if (!statusExists)
                    return BadRequest(new { message = "Ticket status does not exist" });

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating ticket", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            try
            {
                if (id != ticket.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var existingTicket = await _context.Tickets.FindAsync(id);
                if (existingTicket == null)
                    return NotFound(new { message = "Ticket not found" });

                existingTicket.Title = ticket.Title;
                existingTicket.Description = ticket.Description;
                existingTicket.StatusId = ticket.StatusId;

                _context.Entry(existingTicket).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating ticket", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                    return NotFound(new { message = "Ticket not found" });

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting ticket", error = ex.Message });
            }
        }
    }
}
