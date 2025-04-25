using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTO;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Organizador")]  // Comentado para teste sem cookies/JWT
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EventController(ApplicationDbContext context) => _context = context;

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventEntity == null)
                return NotFound(new { message = "Event not found" });

            var eventDto = new EventDto
            {
                Id = eventEntity.Id,
                OrganizerId = eventEntity.OrganizerId,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                EventStartDate = eventEntity.EventStartDate,
                EventEndDate = eventEntity.EventEndDate,
                Location = eventEntity.Location,
                Capacity = eventEntity.Capacity,
                Category = eventEntity.Category
            };
            return Ok(eventDto);
        }

        [HttpGet]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Name = e.Name,
                    Description = e.Description,
                    EventStartDate = e.EventStartDate,
                    EventEndDate = e.EventEndDate,
                    Location = e.Location,
                    Capacity = e.Capacity,
                    Category = e.Category
                })
                .ToListAsync();
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            // Verifica manual do userType vindo no header X-UserType
            if (!Request.Headers.TryGetValue("X-UserType", out var userTypes)
                || userTypes.FirstOrDefault() != "Organizador")
            {
                return Forbid();
            }

            var eventEntity = new Event
            {
                OrganizerId = eventDto.OrganizerId,
                Name = eventDto.Name,
                Description = eventDto.Description,
                EventStartDate = eventDto.EventStartDate,
                EventEndDate = eventDto.EventEndDate,
                Location = eventDto.Location,
                Capacity = eventDto.Capacity,
                Category = eventDto.Category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Evento criado com sucesso!", eventId = eventEntity.Id });
        }

        [HttpPost("participate")]
        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> Participate([FromBody] RegistrationDto registrationDto)
        {
            var evento = await _context.Events.FindAsync(registrationDto.EventId);
            if (evento == null)
                return NotFound(new { message = "Evento não encontrado." });

            registrationDto.TicketId = registrationDto.TicketId == 0 ? 1 : registrationDto.TicketId;
            if (await _context.Registrations.AnyAsync(r => r.UserId == registrationDto.UserId && r.EventId == registrationDto.EventId))
                return BadRequest(new { message = "Já inscrito." });

            var registration = new Registration
            {
                UserId = registrationDto.UserId,
                EventId = registrationDto.EventId,
                TicketId = registrationDto.TicketId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Ativa"
            };
            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Inscrição realizada com sucesso." });
        }

        [HttpGet("participant/{userId}")]
        [Authorize(Roles = "Participante,Organizador")]
        public async Task<IActionResult> GetEventsByParticipant(int userId)
        {
            var registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                .Select(r => new EventDto
                {
                    Id = r.Event.Id,
                    OrganizerId = r.Event.OrganizerId,
                    Name = r.Event.Name,
                    Description = r.Event.Description,
                    EventStartDate = r.Event.EventStartDate,
                    EventEndDate = r.Event.EventEndDate,
                    Location = r.Event.Location,
                    Capacity = r.Event.Capacity,
                    Category = r.Event.Category
                })
                .ToListAsync();

            if (!registrations.Any())
                return NotFound(new { message = "Nenhum evento encontrado." });

            return Ok(registrations);
        }

        [HttpDelete("{eventId}/participants/{userId}")]
        [Authorize(Roles = "Participante")]
        public async Task<IActionResult> CancelParticipation(int eventId, int userId)
        {
            var registration = await _context.Registrations.FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);
            if (registration == null)
                return NotFound(new { message = "Inscrição não encontrada." });

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Participação cancelada com sucesso." });
        }
    }
}
