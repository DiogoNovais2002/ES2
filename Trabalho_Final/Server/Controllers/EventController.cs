using Server.Data;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organizador")]             // só Organizadores podem criar/editar/apagar
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]                             // quem quiser ver detalhes
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventEntity = await _context.Events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

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
        [Authorize(Roles = "Organizador,Participante")] // ambos podem listar
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

        [HttpGet("categories")]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _context.Events
                .Select(e => e.Category)
                .Distinct()
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("localidades")]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<ActionResult<List<string>>> GetLocalidades()
        {
            var localidades = await _context.Events
                .Select(e => e.Location)
                .Distinct()
                .ToListAsync();

            return Ok(localidades);
        }

        [HttpGet("datas")]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<ActionResult<List<string>>> GetDatas()
        {
            var datas = await _context.Events
                .Select(e => e.EventStartDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            var datasFormatadas = datas.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            return Ok(datasFormatadas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
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

            return Ok(new
            {
                message = "Evento criado com sucesso!",
                eventId = eventEntity.Id
            });
        }

        [HttpPost("participate")]
        [Authorize(Roles = "Participante")]            // apenas Participantes
        public async Task<IActionResult> Participate([FromBody] RegistrationDto registrationDto)
        {
            var evento = await _context.Events.FindAsync(registrationDto.EventId);
            if (evento == null)
                return NotFound(new { message = "Evento não encontrado." });

            if (registrationDto.TicketId == 0)
                registrationDto.TicketId = 1; // padrão

            var inscrito = await _context.Registrations
                .AnyAsync(r => r.UserId == registrationDto.UserId && r.EventId == registrationDto.EventId);

            if (inscrito)
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
        [Authorize(Roles = "Participante")]            // apenas o próprio pode cancelar
        public async Task<IActionResult> CancelParticipation(int eventId, int userId)
        {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (registration == null)
                return NotFound(new { message = "Inscrição não encontrada." });

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Participação cancelada com sucesso." });
        }
    }
}
