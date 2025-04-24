using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventService _service;

        public EventController(EventService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventDto = await _service.GetByIdAsync(id);
            if (eventDto == null)
                return NotFound(new { message = "Evento não encontrado." });

            return Ok(eventDto);
        }

        [HttpGet]
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
        public async Task<ActionResult<List<string>>> GetCategories()
        {
            var categories = await _service.GetCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet("localidades")]
        public async Task<ActionResult<List<string>>> GetLocalidades()
        {
            var localidades = await _service.GetLocalidadesAsync();
            return Ok(localidades);
        }
        
        [HttpGet("datas")]
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
    }
}