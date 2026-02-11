using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.AuthDTOs;
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
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
