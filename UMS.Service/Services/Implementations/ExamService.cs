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
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class ExamService : IExamService
    {
        private readonly CustomDBContext _context;
        private readonly IMapper _mapper;

        public ExamService(CustomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExamCreateDTO> CreateAsync(ExamCreateDTO dto)
        {
            var entity = _mapper.Map<Exam>(dto);
            await _context.Set<Exam>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExamCreateDTO>(entity);
        }

        public async Task<ExamGetDTO> GetByIdAsync(int id)
        {
            var query = _context.Set<Exam>()
                        .AsQueryable()
                        .Where(e => !e.IsDeleted && e.Id == id)
                        .AsNoTracking();

            var entity = await query.FirstOrDefaultAsync();
            return entity == null ? null! : _mapper.Map<ExamGetDTO>(entity);
        }

        public async Task<IEnumerable<ExamGetDTO>> GetAllAsync()
        {
            var query = _context.Set<Exam>()
                        .AsQueryable()
                        .Where(e => !e.IsDeleted)
                        .AsNoTracking();

            var list = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ExamGetDTO>>(list);
        }

        public async Task<ExamCreateDTO> UpdateAsync(int id, ExamCreateDTO dto)
        {
            var entity = await _context.Set<Exam>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Exam with id {id} not found.");

            _mapper.Map(dto, entity);

            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<ExamCreateDTO>(entity);
        }

        public async Task<ExamCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Set<Exam>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Exam with id {id} not found.");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<ExamCreateDTO>(entity);
        }

        public async Task<ExamPatchDTO> PatchAsync(int id, ExamPatchDTO dto)
        {
            var entity = await _context.Set<Exam>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);
            _mapper.Map(dto, entity);
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return _mapper.Map<ExamPatchDTO>(entity);
        }
    }
}
