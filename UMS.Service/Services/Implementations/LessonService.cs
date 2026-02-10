using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.LessonDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly CustomDBContext _context;
        private readonly IMapper _mapper;

        public LessonService(CustomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<LessonCreateDTO> CreateAsync(LessonCreateDTO dto)
        {
            var entity = _mapper.Map<Lesson>(dto);

            if (dto.SectionIds?.Any() == true)
            {
                var sections = await _context.Sections
                    .Where(s => dto.SectionIds.Contains(s.Id))
                    .ToListAsync();

                entity.Sections = sections;
            }

            if (dto.TeacherIds?.Any() == true)
            {
                var teachers = await _context.Teachers
                    .Where(s => dto.TeacherIds.Contains(s.Id))
                    .ToListAsync();

                entity.Teachers = teachers;
            }

            await _context.Lessons.AddAsync(entity);
            await _context.SaveChangesAsync();

            var result = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.Sections)
                .Include(l => l.Teachers)
                .FirstOrDefaultAsync(l => l.Id == entity.Id);

            return _mapper.Map<LessonCreateDTO>(result!);
        }

        public async Task<LessonGetDTO> GetByIdAsync(int id)
        {
            var querry = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.Sections)
                .Include(l => l.Teachers)
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            return querry == null ? null! : _mapper.Map<LessonGetDTO>(querry);
        }

        public async Task<IEnumerable<LessonGetDTO>> GetAllAsync()
        {
            var querry = await _context.Lessons
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(l => l.Sections)
                .Include(l => l.Teachers)
                .ToListAsync();

            return _mapper.Map<IEnumerable<LessonGetDTO>>(querry);
        }

        public async Task<LessonCreateDTO> UpdateAsync(int id, LessonCreateDTO dto)
        {
            var entity = await _context.Lessons
                .Include(l => l.Sections)
                .Include(l => l.Teachers)
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null) throw new KeyNotFoundException($"Lesson with id {id} not found.");

            _mapper.Map(dto, entity);

            if (dto.SectionIds != null)
            {
                var sections = await _context.Sections
                    .Where(s => dto.SectionIds.Contains(s.Id))
                    .ToListAsync();

                entity.Sections.Clear();
                foreach (var s in sections) entity.Sections.Add(s);
            }

            if (dto.TeacherIds != null)
            {
                var teachers = await _context.Teachers
                    .Where(s => dto.TeacherIds.Contains(s.Id))
                    .ToListAsync();

                entity.Teachers.Clear();
                foreach (var t in teachers) entity.Teachers.Add(t);
            }

            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var updated = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.Sections)
                .Include(l => l.Teachers)
                .FirstOrDefaultAsync(l => l.Id == id);

            return _mapper.Map<LessonCreateDTO>(updated!);
        }

        public async Task<LessonCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Lessons.FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Lesson with id {id} not found.");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<LessonCreateDTO>(entity);
        }

        public async Task<LessonPatchDTO> PatchAsync(int id, LessonPatchDTO dto)
        {
            var entity = await _context.Set<Lesson>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            _mapper.Map(dto, entity);
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<LessonPatchDTO>(entity);
        }
    }
}
