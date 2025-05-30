using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTO;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Organizador")]
public class OrganizerAnnouncementController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrganizerAnnouncementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: Enviar anúncio
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateAnnouncement([FromBody] OrganizerAnnouncementDto input)
    {

        // 3. Criar e guardar o aviso
        var announcement = new OrganizerAnnouncement
        {
            EventId = input.EventId,
            Title = input.Title,
            Content = input.Content,
            SentAt = DateTime.UtcNow
        };

        _context.OrganizerAnnouncements.Add(announcement);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Anúncio enviado com sucesso!" });
    }




    // GET: Listar anúncios de um evento (visível para todos os participantes)
    [HttpGet("event/{eventId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAnnouncementsForEvent(int eventId)
    {
        var list = await _context.OrganizerAnnouncements
            .Where(a => a.EventId == eventId)
            .OrderByDescending(a => a.SentAt)
            .ToListAsync();

        return Ok(list);
    }
}