using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.StudentDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly CustomDBContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public StudentService(CustomDBContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        public async Task<StudentCreateDTO> CreateAsync(StudentCreateDTO dto)
        {
            var entity = _mapper.Map<Student>(dto);
            await _context.Set<Student>().AddAsync(entity);
            await _context.SaveChangesAsync();

            // ── Add student to section's StudentIds ──
            if (entity.SectionId.HasValue)
            {
                var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == entity.SectionId && !s.IsDeleted);
                if (section != null)
                {
                    section.StudentIds ??= new List<int>();
                    if (!section.StudentIds.Contains(entity.Id))
                        section.StudentIds.Add(entity.Id);
                    section.LastModifiedTime = DateTime.UtcNow;
                }
            }

            // Auto-create a User account for the student
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword($"{dto.Name}{dto.DateOfBirth:yyyyMMdd}"),
                Role = "Student",
                Name = $"{dto.Name} {dto.Surname}",
                StudentId = entity.Id
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentCreateDTO>(entity);
        }

        public async Task<StudentGetDTO> GetByIdAsync(int id)
        {
            var query = _context.Set<Student>()
                        .AsQueryable()
                        .Where(e => !e.IsDeleted && e.Id == id)
                        .AsNoTracking();

            var entity = await query.FirstOrDefaultAsync();
            return entity == null ? null! : _mapper.Map<StudentGetDTO>(entity);
        }

        public async Task<IEnumerable<StudentGetDTO>> GetAllAsync()
        {
            var query = _context.Set<Student>()
                        .Where(e => !e.IsDeleted)
                        .AsNoTracking();

            var list = await query.ToListAsync();
            return _mapper.Map<IEnumerable<StudentGetDTO>>(list);
        }

        public async Task<StudentCreateDTO> UpdateAsync(int id, StudentCreateDTO dto)
        {
            var entity = await _context.Set<Student>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null)
                throw new KeyNotFoundException($"Student with id {id} not found.");

            if (!string.IsNullOrEmpty(entity.PhotoUrl) && entity.PhotoUrl != dto.PhotoUrl)
                _photoService.DeletePhoto(entity.PhotoUrl);

            var oldSectionId = entity.SectionId;

            _mapper.Map(dto, entity);

            // Sync user email if student email changed
            var user = await _context.Users.FirstOrDefaultAsync(u => u.StudentId == id && !u.IsDeleted);
            if (user != null)
            {
                user.Email = dto.Email;
                user.Name = $"{dto.Name} {dto.Surname}";
            }

            // ── Sync Section.StudentIds when class changes ──
            if (oldSectionId != entity.SectionId)
            {
                // Remove from old section's StudentIds
                if (oldSectionId.HasValue)
                {
                    var oldSection = await _context.Sections.FirstOrDefaultAsync(s => s.Id == oldSectionId && !s.IsDeleted);
                    if (oldSection != null)
                    {
                        oldSection.StudentIds?.Remove(id);
                        oldSection.LastModifiedTime = DateTime.UtcNow;
                    }
                }

                // Add to new section's StudentIds
                if (entity.SectionId.HasValue)
                {
                    var newSection = await _context.Sections.FirstOrDefaultAsync(s => s.Id == entity.SectionId && !s.IsDeleted);
                    if (newSection != null)
                    {
                        newSection.StudentIds ??= new List<int>();
                        if (!newSection.StudentIds.Contains(id))
                            newSection.StudentIds.Add(id);
                        newSection.LastModifiedTime = DateTime.UtcNow;
                    }
                }
            }

            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentCreateDTO>(entity);
        }

        public async Task<StudentCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Set<Student>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null) throw new KeyNotFoundException($"Student with id {id} not found.");

            // ── Remove from section's StudentIds ──
            if (entity.SectionId.HasValue)
            {
                var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == entity.SectionId && !s.IsDeleted);
                if (section != null)
                {
                    section.StudentIds?.Remove(id);
                    section.LastModifiedTime = DateTime.UtcNow;
                }
            }

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;

            // Soft-delete the linked user account too
            var user = await _context.Users.FirstOrDefaultAsync(u => u.StudentId == id && !u.IsDeleted);
            if (user != null)
            {
                user.IsDeleted = true;
                user.DeletedTime = DateTime.UtcNow;
            }

            _photoService.DeletePhoto(entity.PhotoUrl);

            await _context.SaveChangesAsync();

            return _mapper.Map<StudentCreateDTO>(entity);
        }

        public async Task<StudentPatchDTO> PatchAsync(int id, StudentPatchDTO dto)
        {
            var entity = await _context.Set<Student>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            _mapper.Map(dto, entity);
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<StudentPatchDTO>(entity);
        }
    }
}
