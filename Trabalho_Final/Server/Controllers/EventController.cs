using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organizador")]
    public class EventController : ControllerBase
    {
        private readonly EventService _service;
        private readonly ApplicationDbContext _context;

        public EventController(EventService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventDto = await _service.GetByIdAsync(id);
            if (eventDto == null)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(eventDto);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _service.GetAllAsync();
            return Ok(events);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEventAsync(int id, [FromBody] EventDto evento)
        {
            var success = await _service.UpdateAsync(id, evento);
            if (!success)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(new { message = "Evento atualizado com sucesso!" });
        }

        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _service.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("localidades")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetLocalidades()
        {
            var localidades = await _service.GetLocalidadesAsync();
            return Ok(localidades);
        }

        [HttpGet("datas")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetDatas()
        {
            var datas = await _service.GetDatasAsync();
            return Ok(datas);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            var eventId = await _service.CreateAsync(eventDto);
            return Ok(new
            {
                message = "Evento criado com sucesso!",
                eventId = eventId
            });
        }

        [HttpPost("participate")]
        [AllowAnonymous]
        public async Task<IActionResult> Participate([FromBody] RegistrationDto registrationDto)
        {
            var result = await _service.ParticipateAsync(registrationDto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("participant/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventsByParticipant(int userId)
        {
            var events = await _service.GetEventsByParticipantAsync(userId);
            if (!events.Any())
                return NotFound(new { message = "Nenhum evento encontrado." });

            return Ok(events);
        }

        [HttpGet("participant/{userId}/registrations")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegistrationsWithTickets(int userId)
        {
            var registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                .Include(r => r.Ticket)
                .Select(r => new
                {
                    EventId = r.Event.Id,
                    EventName = r.Event.Name,
                    TicketId = r.Ticket.Id,
                    TicketType = r.Ticket.TicketType,
                    TicketPrice = r.Ticket.Price,
                    RegistrationDate = r.RegistrationDate
                })
                .ToListAsync();

            return Ok(registrations);
        }

        [HttpDelete("{eventId}/participants/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelParticipation(int eventId, int userId)
        {
            var success = await _service.CancelParticipationAsync(eventId, userId);
            if (!success)
                return NotFound(new { message = "Inscrição não encontrada." });

            return Ok(new { message = "Participação cancelada com sucesso." });
        }

        [HttpGet("EventReport")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReport()
        {
            var report = await _service.GetReportAsync();
            return Ok(report);
        }
    }
}
