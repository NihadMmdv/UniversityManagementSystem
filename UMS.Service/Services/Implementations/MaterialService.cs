using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using UMS.DAL;
using UMS.DAL.Entities;
using UMS.Service.DTOs.MaterialDTOs;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly CustomDBContext _context;
        private readonly string _uploadsFolder;

        public MaterialService(CustomDBContext context, IWebHostEnvironment env)
        {
            _context = context;

            // Store outside wwwroot so files are NOT publicly accessible
            _uploadsFolder = Path.Combine(env.ContentRootPath, "MaterialUploads");
            Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<MaterialGetDTO> UploadAsync(int teacherId, int lessonId, string title, MaterialType type, IFormFile file)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            // Verify teacher teaches this lesson
            var teaches = await _context.Lessons
                .AnyAsync(l => l.Id == lessonId && !l.IsDeleted && l.Teachers.Any(t => t.Id == teacherId));
            if (!teaches)
                throw new UnauthorizedAccessException("You do not teach this lesson.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsFolder, storedFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var entity = new Material
            {
                Title = title,
                FileName = file.FileName,
                StoredFileName = storedFileName,
                Type = type,
                LessonId = lessonId,
                TeacherId = teacherId
            };

            await _context.Materials.AddAsync(entity);
            await _context.SaveChangesAsync();

            var saved = await _context.Materials
                .Include(m => m.Lesson)
                .Include(m => m.Teacher)
                .AsNoTracking()
                .FirstAsync(m => m.Id == entity.Id);

            return MapToDto(saved);
        }

        public async Task<List<MaterialGetDTO>> GetByTeacherAsync(int teacherId)
        {
            var materials = await _context.Materials
                .Where(m => !m.IsDeleted && m.TeacherId == teacherId)
                .Include(m => m.Lesson)
                .Include(m => m.Teacher)
                .AsNoTracking()
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return materials.Select(MapToDto).ToList();
        }

        public async Task<List<MaterialGetDTO>> GetByLessonAsync(int lessonId, bool includeQuestions)
        {
            var query = _context.Materials
                .Where(m => !m.IsDeleted && m.LessonId == lessonId);

            if (!includeQuestions)
                query = query.Where(m => m.Type == MaterialType.Material);

            var materials = await query
                .Include(m => m.Lesson)
                .Include(m => m.Teacher)
                .AsNoTracking()
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return materials.Select(MapToDto).ToList();
        }

        public async Task<(Stream stream, string fileName, string contentType)?> DownloadAsync(int id)
        {
            var material = await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (material == null) return null;

            var filePath = Path.Combine(_uploadsFolder, material.StoredFileName);
            if (!File.Exists(filePath)) return null;

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(material.FileName, out var contentType))
                contentType = "application/octet-stream";

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, material.FileName, contentType);
        }

        public async Task<bool> DeleteAsync(int id, int teacherId)
        {
            var entity = await _context.Materials
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (entity == null) return false;

            // teacherId == 0 means admin (can delete any)
            if (teacherId != 0 && entity.TeacherId != teacherId) return false;

            // Delete file from disk
            var filePath = Path.Combine(_uploadsFolder, entity.StoredFileName);
            if (File.Exists(filePath)) File.Delete(filePath);

            entity.IsDeleted = true;
            entity.DeletedTime = DateTime.UtcNow;
            entity.LastModifiedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Material?> GetEntityByIdAsync(int id)
        {
            return await _context.Materials
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        }

        private static MaterialGetDTO MapToDto(Material m) => new()
        {
            Id = m.Id,
            Title = m.Title,
            FileName = m.FileName,
            Type = m.Type.ToString(),
            LessonId = m.LessonId,
            LessonName = m.Lesson?.Name ?? "",
            TeacherId = m.TeacherId,
            TeacherName = m.Teacher != null ? $"{m.Teacher.Name} {m.Teacher.Surname}" : ""
        };
    }
}