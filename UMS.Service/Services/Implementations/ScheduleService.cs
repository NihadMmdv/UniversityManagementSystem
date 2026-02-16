using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ScheduleDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly CustomDBContext _context;
        private readonly IMapper _mapper;

        public ScheduleService(CustomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ScheduleCreateDTO> CreateAsync(ScheduleCreateDTO dto)
        {
            await ValidateTeachersForLesson(dto.LessonId, dto.TeacherIds);

            var entity = _mapper.Map<Schedule>(dto);

            if (dto.TeacherIds.Count > 0)
            {
                entity.Teachers = await _context.Teachers
                    .Where(t => dto.TeacherIds.Contains(t.Id))
                    .ToListAsync();
            }

            await _context.Set<Schedule>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ScheduleCreateDTO>(entity);
        }

        public async Task<ScheduleGetDTO> GetByIdAsync(int id)
        {
            var entity = await _context.Set<Schedule>()
                        .Include(s => s.Teachers)
                        .Where(e => !e.IsDeleted && e.Id == id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
            return entity == null ? null! : _mapper.Map<ScheduleGetDTO>(entity);
        }

        public async Task<IEnumerable<ScheduleGetDTO>> GetAllAsync()
        {
            var list = await _context.Set<Schedule>()
                        .Include(s => s.Teachers)
                        .Where(e => !e.IsDeleted)
                        .AsNoTracking()
                        .ToListAsync();
            return _mapper.Map<IEnumerable<ScheduleGetDTO>>(list);
        }

        public async Task<ScheduleCreateDTO> UpdateAsync(int id, ScheduleCreateDTO dto)
        {
            await ValidateTeachersForLesson(dto.LessonId, dto.TeacherIds);

            var entity = await _context.Set<Schedule>()
                .Include(s => s.Teachers)
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Schedule with id {id} not found.");

            _mapper.Map(dto, entity);

            entity.Teachers.Clear();
            if (dto.TeacherIds.Count > 0)
            {
                entity.Teachers = await _context.Teachers
                    .Where(t => dto.TeacherIds.Contains(t.Id))
                    .ToListAsync();
            }

            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<ScheduleCreateDTO>(entity);
        }

        public async Task<ScheduleCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Set<Schedule>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null)
                throw new KeyNotFoundException($"Schedule with id {id} not found.");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<ScheduleCreateDTO>(entity);
        }

        private async Task ValidateTeachersForLesson(int lessonId, List<int> teacherIds)
        {
            if (teacherIds == null || teacherIds.Count == 0)
                return;

            var validTeacherIds = await _context.Teachers
                .Where(t => t.Lessons.Any(l => l.Id == lessonId))
                .Select(t => t.Id)
                .ToListAsync();

            var invalidIds = teacherIds.Except(validTeacherIds).ToList();
            if (invalidIds.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Teachers with IDs [{string.Join(", ", invalidIds)}] are not assigned to lesson {lessonId}.");
            }
        }
    }
}