using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UMS.DAL.Entities;
using UMS.Service.DTOs.MaterialDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<MaterialGetDTO> UploadAsync(int teacherId, int lessonId, string title, MaterialType type, IFormFile file);
        Task<List<MaterialGetDTO>> GetByTeacherAsync(int teacherId);
        Task<List<MaterialGetDTO>> GetByLessonAsync(int lessonId, bool includeQuestions);
        Task<(Stream stream, string fileName, string contentType)?> DownloadAsync(int id);
        Task<bool> DeleteAsync(int id, int teacherId);
        Task<Material?> GetEntityByIdAsync(int id);
    }
}