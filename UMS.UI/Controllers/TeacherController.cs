using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.TeacherDTOs;
using UMS.Service.Services.Implementations;
using UMS.Service.Services.Interfaces;

namespace UMS.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherGetDTO>>> GetAll()
        {
            var list = await _teacherService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherGetDTO>> Get(int id)
        {
            var dto = await _teacherService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherCreateDTO>> Create([FromBody] TeacherCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _teacherService.CreateAsync(dto);

            return Created(string.Empty, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TeacherCreateDTO>> Update(int id, [FromBody] TeacherCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _teacherService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TeacherCreateDTO>> Delete(int id)
        {
            var deleted = await _teacherService.DeleteAsync(id);
            return Ok(deleted);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, TeacherPatchDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _teacherService.PatchAsync(id, dto);
            return Ok(updated);
        }
    }
}