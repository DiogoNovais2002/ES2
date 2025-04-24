using Server.Data;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTO;

[ApiController]
[Route("api/[controller]")]
public class EventTicketController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventTicketController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/EventTicket/{eventId}
    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetTicketsForEvent(int eventId)
    {
        var tickets = await _context.EventTickets
            .Where(t => t.EventId == eventId)
            .Select(t => new EventTicketDto
            {
                Id = t.Id,
                EventId = t.EventId,
                TicketType = t.TicketType,
                Price = t.Price,
                QuantityAvailable = t.QuantityAvailable,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(tickets);
    }

    // POST: api/EventTicket
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] EventTicketDto dto)
    {
        var ticket = new EventTicket
        {
            EventId = dto.EventId,
            TicketType = dto.TicketType,
            Price = dto.Price,
            QuantityAvailable = dto.QuantityAvailable,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.EventTickets.Add(ticket);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Bilhete criado com sucesso!", ticket.Id });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket(int id, [FromBody] EventTicketDto dto)
    {
        var ticket = await _context.EventTickets.FindAsync(id);
        if (ticket == null)
            return NotFound(new { message = "Bilhete não encontrado." });

        ticket.TicketType = dto.TicketType;
        ticket.Price = dto.Price;
        ticket.QuantityAvailable = dto.QuantityAvailable;
        ticket.Description = dto.Description;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Bilhete atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.EventTickets.FindAsync(id);
        if (ticket == null)
            return NotFound(new { message = "Bilhete não encontrado." });

        _context.EventTickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Bilhete eliminado com sucesso." });
    }

}