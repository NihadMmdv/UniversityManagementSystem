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

        public StudentService(CustomDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<StudentCreateDTO> CreateAsync(StudentCreateDTO dto)
        {
            var entity = _mapper.Map<Student>(dto);
            await _context.Set<Student>().AddAsync(entity);
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
                        .AsQueryable()
                        .Where(e => !e.IsDeleted)
                        .AsNoTracking();

            var list = await query.ToListAsync();
            return _mapper.Map<IEnumerable<StudentGetDTO>>(list);
        }

        public async Task<StudentCreateDTO> UpdateAsync(int id, StudentCreateDTO dto)
        {
            var entity = await _context.Set<Student>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            _mapper.Map(dto, entity);

            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<StudentCreateDTO>(entity);
        }

        public async Task<StudentCreateDTO> DeleteAsync(int id)
        {
            var entity = await _context.Set<Student>().FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (entity == null) throw new KeyNotFoundException($"Student with id {id} not found.");

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;

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
