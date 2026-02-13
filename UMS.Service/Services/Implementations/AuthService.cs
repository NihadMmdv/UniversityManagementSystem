using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.AuthDTOs;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.LessonDTOs;
using UMS.Service.DTOs.ScheduleDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly CustomDBContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(CustomDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return new AuthResponseDTO
            {
                Token = GenerateJwtToken(user),
                Email = user.Email,
                Role = user.Role,
                Name = user.Name ?? ""
            };
        }

        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Name = dto.Name,
                Role = "User" // Default role
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Token = GenerateJwtToken(user),
                Email = user.Email,
                Role = user.Role,
                Name = user.Name ?? ""
            };
        }

        public async Task<ProfileDTO?> GetProfileAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Student).ThenInclude(s => s!.Section)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null) return null;

            var profile = new ProfileDTO
            {
                Email = user.Email,
                Role = user.Role
            };

            if (user.Student != null)
            {
                profile.Name = user.Student.Name;
                profile.Surname = user.Student.Surname;
                profile.Phone = user.Student.Phone;
                profile.PhotoUrl = user.Student.PhotoUrl;
                profile.DateOfBirth = user.Student.DateOfBirth;
                profile.ClassName = user.Student.Section?.Name;
            }
            else if (user.Teacher != null)
            {
                profile.Name = user.Teacher.Name;
                profile.Surname = user.Teacher.Surname;
                profile.Phone = user.Teacher.Phone;
                profile.PhotoUrl = user.Teacher.PhotoUrl;
                profile.DateOfBirth = user.Teacher.DateOfBirth;
            }
            else
            {
                profile.Name = user.Name ?? "";
            }

            return profile;
        }

        public async Task<List<LessonGetDTO>> GetMyLessonsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s!.Section)
                        .ThenInclude(sec => sec!.Lessons)
                            .ThenInclude(l => l.Teachers)
                .Include(u => u.Student)
                    .ThenInclude(s => s!.Section)
                        .ThenInclude(sec => sec!.Lessons)
                            .ThenInclude(l => l.Sections)
                .Include(u => u.Teacher)
                    .ThenInclude(t => t!.Lessons)
                        .ThenInclude(l => l.Teachers)
                .Include(u => u.Teacher)
                    .ThenInclude(t => t!.Lessons)
                        .ThenInclude(l => l.Sections)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                return new List<LessonGetDTO>();

            List<Lesson> lessons;

            if (user.Teacher?.Lessons != null)
                lessons = user.Teacher.Lessons;
            else if (user.Student?.Section?.Lessons != null)
                lessons = user.Student.Section.Lessons;
            else
                return new List<LessonGetDTO>();

            return lessons.Select(l => new LessonGetDTO
            {
                Id = l.Id,
                Name = l.Name,
                SectionIds = l.Sections?.Select(s => s.Id).ToList() ?? new List<int>(),
                TeacherIds = l.Teachers?.Select(t => t.Id).ToList() ?? new List<int>()
            }).ToList();
        }

        public async Task<List<MyExamDTO>> GetMyExamsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                    .ThenInclude(s => s!.Exams)
                        .ThenInclude(e => e.Lesson)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user?.Student?.Exams == null)
                return new List<MyExamDTO>();

            return user.Student.Exams.Select(e => new MyExamDTO
            {
                Id = e.Id,
                LessonName = e.Lesson?.Name ?? "Unknown",
                ExamDate = e.ExamDate,
                Score = e.Score
            }).ToList();
        }

        public async Task<List<ScheduleEntryDTO>> GetMyScheduleAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
                return new List<ScheduleEntryDTO>();

            IQueryable<Schedule> query = _context.Schedules
                .Include(sc => sc.Lesson)
                .Include(sc => sc.Section)
                .Include(sc => sc.Teachers)
                .Where(sc => !sc.IsDeleted);

            if (user.Teacher != null)
            {
                // Only show schedules where this teacher is assigned
                query = query.Where(sc => sc.Teachers.Any(t => t.Id == user.TeacherId));
            }
            else if (user.Student?.SectionId != null)
            {
                query = query.Where(sc => sc.SectionId == user.Student.SectionId);
            }
            else
            {
                return new List<ScheduleEntryDTO>();
            }

            var rows = await query
                .OrderBy(sc => sc.DayOfWeek)
                .ThenBy(sc => sc.StartTime)
                .ToListAsync();

            return rows.Select(sc => new ScheduleEntryDTO
            {
                LessonName = sc.Lesson.Name,
                SectionName = sc.Section.Name,
                TeacherNames = sc.Teachers.Select(t => $"{t.Name} {t.Surname}".Trim()).ToList(),
                DayOfWeek = sc.DayOfWeek,
                StartTime = sc.StartTime.ToString("HH:mm"),
                EndTime = sc.EndTime.ToString("HH:mm")
            }).ToList();
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
