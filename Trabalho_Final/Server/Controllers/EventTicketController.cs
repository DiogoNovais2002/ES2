using Server.Data;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTO;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Organizador")]   // só Organizadores podem criar/remover bilhetes
public class EventTicketController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventTicketController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{eventId}")]
    [AllowAnonymous]
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

    [HttpPost]
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
    public async Task<IActionResult> Delete(int id)
    {
        var ticket = await _context.EventTickets.FindAsync(id);
        if (ticket == null)
            return NotFound(new { message = "Bilhete não encontrado." });

        _context.EventTickets.Remove(ticket);
        await _context.SaveChangesAsync();


        return Ok(new { message = "Bilhete eliminado com sucesso." });
    }
}
