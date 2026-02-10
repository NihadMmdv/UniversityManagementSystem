using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.LessonDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface ILessonService
    {
        Task<LessonCreateDTO> CreateAsync(LessonCreateDTO dto);
        Task<LessonGetDTO> GetByIdAsync(int id);
        Task<IEnumerable<LessonGetDTO>> GetAllAsync();
        Task<LessonCreateDTO> UpdateAsync(int id, LessonCreateDTO dto);
        Task<LessonCreateDTO> DeleteAsync(int id);
        Task<LessonPatchDTO> PatchAsync(int id, LessonPatchDTO dto);
    }
}
