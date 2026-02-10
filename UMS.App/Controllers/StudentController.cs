using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Service.DTOs.StudentDTOs;
using UMS.Service.Services.Implementations;
using UMS.Service.Services.Interfaces;

namespace UMS.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _StudentService;

        public StudentController(IStudentService StudentService)
        {
            _StudentService = StudentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentGetDTO>>> GetAll()
        {
            var list = await _StudentService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentGetDTO>> Get(int id)
        {
            var dto = await _StudentService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<StudentCreateDTO>> Create([FromBody] StudentCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _StudentService.CreateAsync(dto);

            return Created(string.Empty, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<StudentCreateDTO>> Update(int id, [FromBody] StudentCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _StudentService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<StudentCreateDTO>> Delete(int id)
        {
            var deleted = await _StudentService.DeleteAsync(id);
            return Ok(deleted);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, StudentPatchDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _StudentService.PatchAsync(id, dto);
            return Ok(updated);
        }

    }
}