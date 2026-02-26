using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.Services.Interfaces;

namespace UMS.UI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly CustomDBContext _context;

        public MaterialController(IMaterialService materialService, CustomDBContext context)
        {
            _materialService = materialService;
            _context = context;
        }

        /// <summary>Upload a material or question file (Teacher only).</summary>
        [HttpPost("upload")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Upload()
        {
            var teacher = await GetTeacherForCurrentUser();
            if (teacher == null) return Forbid();

            var form = await Request.ReadFormAsync();
            const long MaxFileSize = 5L * 1024 * 1024;

            var title = form["title"].ToString();
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest(new { error = "Title is required." });

            if (!int.TryParse(form["lessonId"], out var lessonId))
                return BadRequest(new { error = "Invalid lesson ID." });

            if (!int.TryParse(form["type"], out var type))
                return BadRequest(new { error = "Invalid type." });

            var file = form.Files.GetFile("file");
            if (file is null || file.Length == 0)
                return BadRequest(new { error = "No file provided or file is empty." });
            else if (file.Length > MaxFileSize)
                return BadRequest(new { error = "File size is larger than 5Mb." });

                var result = await _materialService.UploadAsync(
                    teacher.Id, lessonId, title, (MaterialType)type, file);
            return Ok(result);
        }

        /// <summary>List all materials uploaded by the current teacher.</summary>
        [HttpGet("my")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMy()
        {
            var teacher = await GetTeacherForCurrentUser();
            if (teacher == null) return Forbid();

            var materials = await _materialService.GetByTeacherAsync(teacher.Id);
            return Ok(materials);
        }

        /// <summary>List materials for a given lesson (students see only Material type).</summary>
        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetByLesson(int lessonId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Student")
            {
                var student = await GetStudentForCurrentUser();
                if (student == null) return Forbid();

                var enrolled = await _context.Lessons
                    .AnyAsync(l => l.Id == lessonId && !l.IsDeleted &&
                                   l.Sections.Any(s => s.Id == student.SectionId));
                if (!enrolled) return Forbid();
            }

            var materials = await _materialService.GetByLessonAsync(lessonId, userRole != "Student");
            return Ok(materials);
        }

        /// <summary>Download a single material file.</summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var material = await _context.Materials
                .Include(m => m.Lesson)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (material == null) return NotFound();

            if (userRole == "Student")
            {
                if (material.Type == MaterialType.Question)
                    return Forbid();

                var student = await GetStudentForCurrentUser();
                if (student == null) return Forbid();

                var enrolled = await _context.Lessons
                    .AnyAsync(l => l.Id == material.LessonId && !l.IsDeleted &&
                                   l.Sections.Any(s => s.Id == student.SectionId));
                if (!enrolled) return Forbid();
            }
            else if (userRole == "Teacher")
            {
                var teacher = await GetTeacherForCurrentUser();
                if (teacher == null || material.TeacherId != teacher.Id) return Forbid();
            }

            var result = await _materialService.DownloadAsync(id);
            if (result is null) return NotFound();

            var (stream, fileName, contentType) = result.Value;
            return File(stream, contentType, fileName);
        }

        /// <summary>Delete a material (owner teacher or admin).</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var material = await _context.Materials.FindAsync(id);
            if (material == null || material.IsDeleted) return NotFound();

            if (userRole == "Teacher")
            {
                var teacher = await GetTeacherForCurrentUser();
                if (teacher == null) return Forbid();
                if (material.TeacherId != teacher.Id) return Forbid();
            }

            await _materialService.DeleteAsync(id, material.TeacherId);
            return Ok(new { message = "Deleted" });
        }

        // ─── helpers ───
        private async Task<Teacher?> GetTeacherForCurrentUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.Teacher;
        }

        private async Task<Student?> GetStudentForCurrentUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Student)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.Student;
        }
    }
}