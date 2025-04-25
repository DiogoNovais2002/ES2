using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Services;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organizador")]             // só Organizadores podem criar/editar/apagar
    public class EventController : ControllerBase
    {
        private readonly EventService _service;

        public EventController(EventService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]                             // quem quiser ver detalhes
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventDto = await _service.GetByIdAsync(id);
            if (eventDto == null)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(eventDto);
        }

        [HttpGet]
        [Authorize(Roles = "Organizador,Participante")] // ambos podem listar
        public async Task<IActionResult> GetEvents()
        {
            var events = await _service.GetAllAsync();
            return Ok(events);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventAsync(int id, [FromBody] EventDto evento)
        {
            var success = await _service.UpdateAsync(id, evento);
            if (!success)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(new { message = "Evento atualizado com sucesso!" });
        }

        
        [HttpGet("categories")]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _service.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("localidades")]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<ActionResult<List<string>>> GetLocalidades()
        {
            var localidades = await _service.GetLocalidadesAsync();
            return Ok(localidades);
        }

        [HttpGet("datas")]
        [Authorize(Roles = "Organizador,Participante")]
        public async Task<ActionResult<List<string>>> GetDatas()
        {
            var datas = await _service.GetDatasAsync();

            return Ok(datas);
        }

        [HttpPost]
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
