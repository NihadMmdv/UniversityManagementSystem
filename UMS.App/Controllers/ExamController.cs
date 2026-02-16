using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.StudentDTOs;
using UMS.Service.Services.Implementations;
using UMS.Service.Services.Interfaces;

namespace UMS.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamGetDTO>>> GetAll()
        {
            var list = await _examService.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExamGetDTO>> Get(int id)
        {
            var dto = await _examService.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<ExamCreateDTO>> Create([FromBody] ExamCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _examService.CreateAsync(dto);

            return Created(string.Empty, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExamCreateDTO>> Update(int id, [FromBody] ExamCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _examService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ExamCreateDTO>> Delete(int id)
        {
            var deleted = await _examService.DeleteAsync(id);
            return Ok(deleted);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, ExamPatchDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _examService.PatchAsync(id, dto);
            return Ok(updated);
        }
    }
}