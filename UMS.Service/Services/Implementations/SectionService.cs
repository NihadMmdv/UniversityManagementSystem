using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.SectionDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class SectionService : ISectionService
    {
        private readonly CustomDBContext _context;
        private readonly IMapper _mapper;

        public SectionService(CustomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SectionCreateDTO> CreateAsync(SectionCreateDTO dto)
        {
            var entity = _mapper.Map<Section>(dto);

            if (dto.LessonIds?.Any() == true)
            {
                var lessons = await _context.Lessons
                    .Where(l => dto.LessonIds.Contains(l.Id))
                    .ToListAsync();

                entity.Lessons = lessons;
            }

            await _context.Sections.AddAsync(entity);
            await _context.SaveChangesAsync();

            var result = await _context.Sections
                .AsNoTracking()
                .Include(s => s.Lessons)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(s => s.Id == entity.Id);

            return _mapper.Map<SectionCreateDTO>(result!);
        }

        public async Task<SectionGetDTO> GetByIdAsync(int id)
        {
            var query = await _context.Sections
                        .AsNoTracking()
                        .Include(s => s.Lessons)
                        .Include(s => s.Students)
                        .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            return query == null ? null! : _mapper.Map<SectionGetDTO>(query);
        }

        public async Task<IEnumerable<SectionGetDTO>> GetAllAsync()
        {
            var query = await _context.Sections
                        .AsNoTracking()
                        .Where(e => !e.IsDeleted)
                        .Include(s => s.Lessons)
                        .Include(s => s.Students)
                        .ToListAsync();

            return _mapper.Map<IEnumerable<SectionGetDTO>>(query);
        }

        public async Task<SectionCreateDTO> UpdateAsync(int id, SectionCreateDTO dto)
        {
            var entity = await _context.Sections
                .Include(s => s.Lessons)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null) throw new KeyNotFoundException($"Section with id {id} not found.");

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

            var updated = await _context.Sections
                .AsNoTracking()
                .Include(s => s.Lessons)
                .Include(s => s.Students)
                .FirstOrDefaultAsync(s => s.Id == id);

            return _mapper.Map<SectionCreateDTO>(updated!);
        }

        public async Task<SectionCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Sections.FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Section with id {id} not found.");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<SectionCreateDTO>(entity);
        }

        public async Task<SectionPatchDTO> PatchAsync(int id, SectionPatchDTO dto)
        {
            var entity = await _context.Set<Section>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            _mapper.Map(dto, entity);
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<SectionPatchDTO>(entity);
        }
    }
}
