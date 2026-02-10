using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.LessonDTOs;
using UMS.Service.Services.Implementations;
using UMS.Service.Services.Interfaces;

namespace UMS.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _LessonService;

        public LessonsController(ILessonService LessonService)
        {
            _LessonService = LessonService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonGetDTO>>> GetAll()
        {
            var list = await _LessonService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonGetDTO>> Get(int id)
        {
            var dto = await _LessonService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<LessonCreateDTO>> Create([FromBody] LessonCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _LessonService.CreateAsync(dto);

            return Created(string.Empty, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<LessonCreateDTO>> Update(int id, [FromBody] LessonCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _LessonService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<LessonCreateDTO>> Delete(int id)
        {
            var deleted = await _LessonService.DeleteAsync(id);
            return Ok(deleted);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, LessonPatchDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _LessonService.PatchAsync(id, dto);
            return Ok(updated);
        }
    }
}