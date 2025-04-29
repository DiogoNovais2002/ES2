using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using Server.Services;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organizador")]   // só Organizadores podem gerir atividades
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _service;

        public ActivityController(ActivityService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _service.GetAllAsync();
            return Ok(activities);
        }

        [HttpGet("event/{eventId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActivitiesByEventId(int eventId)
        {
            var activities = await _service.GetByEventIdAsync(eventId);
            return Ok(activities);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityDto dto)
        {
            await _service.CreateAsync(dto);
            return Ok();
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(new { message = "Atividade não encontrada." });
                
            return Ok(new { message = "Atividade atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Atividade não encontrada." });

            return Ok(new { message = "Atividade eliminada com sucesso." });
        }
    }
}