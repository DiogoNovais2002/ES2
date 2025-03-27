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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventEntity = await _context.Events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (eventEntity == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            var eventDto = new EventDto
            {
                Id = eventEntity.Id,
                OrganizerId = eventEntity.OrganizerId,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                EventDate = eventEntity.EventDate,
                Location = eventEntity.Location,
                Capacity = eventEntity.Capacity,
                Price = eventEntity.Price,
                Category = eventEntity.Category
            };

            return Ok(eventDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Name = e.Name,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Capacity = e.Capacity,
                    Price = e.Price,
                    Category = e.Category
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            var eventEntity = new Event
            {
                OrganizerId = eventDto.OrganizerId,
                Name = eventDto.Name,
                Description = eventDto.Description,
                EventDate = eventDto.EventDate,
                Location = eventDto.Location,
                Capacity = eventDto.Capacity,
                Price = eventDto.Price,
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
    }
}