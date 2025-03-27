using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2.Data;
using Server.DTO;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os eventos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    id = e.Id,
                    organizerid = e.OrganizerId,
                    name = e.Name,
                    description = e.Description,
                    eventdate = e.EventDate,
                    locacion = e.Location,
                    capacity = e.Capacity,
                    price = e.Price,
                    category = e.Category,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            return Ok(events);
        }

        /// <summary>
        /// Retorna um evento por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var e = await _context.Events.FindAsync(id);

            if (e == null)
                return NotFound(new { message = "Evento não encontrado." });

            var dto = new EventDto
            {
                id = e.Id,
                organizerid = e.OrganizerId,
                name = e.Name,
                description = e.Description,
                eventdate = e.EventDate,
                locacion = e.Location,
                capacity = e.Capacity,
                price = e.Price,
                category = e.Category,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cria um novo evento.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            // Verifica se o User (organizador) existe
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.organizerid);
            if (!userExists)
            {
                return BadRequest(new { message = "Organizador (User) não encontrado." });
            }

            var newEvent = new Event
            {
                OrganizerId = dto.organizerid,
                Name = dto.name,
                Description = dto.description,
                EventDate = dto.eventdate,
                Location = dto.locacion,
                Capacity = dto.capacity,
                Price = dto.price,
                Category = dto.category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            dto.id = newEvent.Id;
            dto.CreatedAt = newEvent.CreatedAt;
            dto.UpdatedAt = newEvent.UpdatedAt;

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, dto);
        }


        /// <summary>
        /// Atualiza um evento existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto dto)
        {
            // Verifica se o evento existe
            var e = await _context.Events.FindAsync(id);
            if (e == null)
                return NotFound(new { message = "Evento não encontrado." });

            // Verifica se o organizador (User) existe
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.organizerid);
            if (!userExists)
                return BadRequest(new { message = "Organizador (User) não encontrado." });

            // Atualiza os dados
            e.OrganizerId = dto.organizerid;
            e.Name = dto.name;
            e.Description = dto.description;
            e.EventDate = dto.eventdate;
            e.Location = dto.locacion;
            e.Capacity = dto.capacity;
            e.Price = dto.price;
            e.Category = dto.category;
            e.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Elimina um evento.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var e = await _context.Events.FindAsync(id);
            if (e == null)
                return NotFound(new { message = "Evento não encontrado." });

            _context.Events.Remove(e);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
