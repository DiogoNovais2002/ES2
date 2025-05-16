using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Services;
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

        // GET /api/Event/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventDto = await _service.GetByIdAsync(id);
            if (eventDto == null)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(eventDto);
        }

        // GET /api/Event
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _service.GetAllAsync();
            return Ok(events);
        }

        // POST /api/Event
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            var eventId = await _service.CreateAsync(eventDto);
            return Ok(new
            {
                message = "Evento criado com sucesso!",
                eventId
            });
        }

        // PUT /api/Event/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEventAsync(int id, [FromBody] EventDto evento)
        {
            var success = await _service.UpdateAsync(id, evento);
            if (!success)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(new { message = "Evento atualizado com sucesso!" });
        }

        // DELETE participação: DELETE /api/Event/{eventId}/participants/{userId}
        [HttpDelete("{eventId}/participants/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelParticipation(int eventId, int userId)
        {
            var success = await _service.CancelParticipationAsync(eventId, userId);
            if (!success)
                return NotFound(new { message = "Inscrição não encontrada." });

            return Ok(new { message = "Participação cancelada com sucesso." });
        }

        // POST participação: POST /api/Event/participate
        [HttpPost("participate")]
        [AllowAnonymous]
        public async Task<IActionResult> Participate([FromBody] RegistrationDto registrationDto)
        {
            var result = await _service.ParticipateAsync(registrationDto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        // GET /api/Event/participant/{userId}
        [HttpGet("participant/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventsByParticipant(int userId)
        {
            var events = await _service.GetEventsByParticipantAsync(userId);
            if (!events.Any())
                return NotFound(new { message = "Nenhum evento encontrado." });

            return Ok(events);
        }

        // ✅ NOVO ENDPOINT: /api/Event/participant/{userId}/registrations
        [HttpGet("participant/{userId}/registrations")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRegistrationsWithTickets(int userId)
        {
            var registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                .ThenInclude(e => e.Activities)
                .Include(r => r.Ticket)
                .Select(r => new
                {
                    EventId = r.Event.Id,
                    EventName = r.Event.Name,
                    Activities = r.Event.Activities.Select(a => new
                    {
                        a.Id,
                        a.Name,
                        a.Description,
                        a.ActivityStartDate,
                        a.ActivityEndDate
                    }).ToList(),
                    TicketId = r.Ticket.Id,
                    TicketType = r.Ticket.TicketType,
                    TicketPrice = r.Ticket.Price,
                    RegistrationDate = r.RegistrationDate
                })
                .ToListAsync();

            return Ok(registrations);
        }


        // GET /api/Event/categories
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _service.GetCategoriesAsync();
            return Ok(categories);
        }

        // GET /api/Event/localidades
        [HttpGet("localidades")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetLocalidades()
        {
            var localidades = await _service.GetLocalidadesAsync();
            return Ok(localidades);
        }

        // GET /api/Event/datas
        [HttpGet("datas")]
        [AllowAnonymous]
        public async Task<ActionResult<List<string>>> GetDatas()
        {
            var datas = await _service.GetDatasAsync();
            return Ok(datas);
        }

        // GET /api/Event/EventReport
        [HttpGet("EventReport")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGeneralReport()
        {
            var report = await _service.GetReportAsync();
            return Ok(report);
        }

        // GET /api/Event/EventReport/{eventId}
        [HttpGet("EventReport/{eventId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReportByEvent(int eventId)
        {
            var detail = await _service.GetReportByEventAsync(eventId);
            if (detail == null)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(detail);
        }
    }
}
