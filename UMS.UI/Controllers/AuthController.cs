using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UMS.Service.DTOs.AuthDTOs;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.LessonDTOs;
using UMS.Service.DTOs.ScheduleDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Delete cookie at the current explicit path
            Response.Cookies.Delete("swagger_token", new CookieOptions
            {
                Path = "/",
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict
            });
            // Delete legacy cookie the browser scoped to /swagger
            Response.Cookies.Delete("swagger_token", new CookieOptions
            {
                Path = "/swagger",
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict
            });
            return Ok(new { message = "Logged out" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null)
                return BadRequest(new { message = "Email already exists" });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _authService.GetProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [Authorize]
        [HttpGet("me/lessons")]
        public async Task<ActionResult<List<LessonGetDTO>>> GetMyLessons()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var lessons = await _authService.GetMyLessonsAsync(userId);
            return Ok(lessons);
        }

        [Authorize]
        [HttpGet("me/exams")]
        public async Task<ActionResult<List<MyExamDTO>>> GetMyExams()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var exams = await _authService.GetMyExamsAsync(userId);
            return Ok(exams);
        }

        [Authorize]
        [HttpGet("me/schedule")]
        public async Task<ActionResult<List<ScheduleEntryDTO>>> GetMySchedule()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var schedule = await _authService.GetMyScheduleAsync(userId);
            return Ok(schedule);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _authService.ChangePasswordAsync(userId, dto);
            if (!success)
                return BadRequest(new { message = "Current password is incorrect" });

            return Ok(new { message = "Password changed successfully" });
        }
    }
}