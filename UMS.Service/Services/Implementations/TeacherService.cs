using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.TeacherDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly CustomDBContext _context;
        private readonly IMapper _mapper;

        public TeacherService(CustomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TeacherCreateDTO> CreateAsync(TeacherCreateDTO dto)
        {
            var entity = _mapper.Map<Teacher>(dto);

            if (dto.LessonIds?.Any() == true)
            {
                var lessons = await _context.Lessons
                                    .Where(l => dto.LessonIds.Contains(l.Id))
                                    .ToListAsync();
                entity.Lessons = lessons;
            }
            await _context.Set<Teacher>().AddAsync(entity);
            await _context.SaveChangesAsync();

            var resuly = await _context.Teachers
                .AsNoTracking()
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(t => t.Id == entity.Id);
            return _mapper.Map<TeacherCreateDTO>(entity);
        }

        public async Task<TeacherGetDTO> GetByIdAsync(int id)
        {
            var query = await _context.Teachers
                .AsNoTracking()
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            return query == null ? null! : _mapper.Map<TeacherGetDTO>(query);
        }

        public async Task<IEnumerable<TeacherGetDTO>> GetAllAsync()
        {
            var query = await _context.Teachers
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(t => t.Lessons)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TeacherGetDTO>>(query);
        }

        public async Task<TeacherCreateDTO> UpdateAsync(int id, TeacherCreateDTO dto)
        {
            var entity = await _context.Teachers
                .Include(l => l.Lessons)
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null) throw new KeyNotFoundException($"Teacher with id {id} not found.");

            _mapper.Map(dto, entity);

            if (dto.LessonIds != null)
            {
                var lessons = await _context.Lessons
                    .Where(l => dto.LessonIds.Contains(l.Id))
                    .ToListAsync();

                entity.Lessons.Clear();
                foreach (var l in lessons) entity.Lessons.Add(l);
            }

            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var updated = await _context.Teachers
                .AsNoTracking()
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(t => t.Id == id);

            return _mapper.Map<TeacherCreateDTO>(entity);
        }

        public async Task<TeacherCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Set<Teacher>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<TeacherCreateDTO>(entity);
        }

        public async Task<TeacherPatchDTO> PatchAsync(int id, TeacherPatchDTO dto)
        {
            var entity = await _context.Set<Teacher>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            _mapper.Map(dto, entity);
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<TeacherPatchDTO>(entity);
        }
    }
}
