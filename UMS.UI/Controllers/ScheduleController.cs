using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Service.DTOs.ScheduleDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleGetDTO>>> GetAll()
        {
            var list = await _scheduleService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ScheduleGetDTO>> Get(int id)
        {
            var dto = await _scheduleService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleCreateDTO>> Create([FromBody] ScheduleCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _scheduleService.CreateAsync(dto);

            return Created(string.Empty, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ScheduleCreateDTO>> Update(int id, [FromBody] ScheduleCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _scheduleService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ScheduleCreateDTO>> Delete(int id)
        {
            var deleted = await _scheduleService.DeleteAsync(id);
            return Ok(deleted);
        }
    }
}