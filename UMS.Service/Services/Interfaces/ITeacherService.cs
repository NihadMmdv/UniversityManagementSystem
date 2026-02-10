using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.TeacherDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherCreateDTO> CreateAsync(TeacherCreateDTO dto);
        Task<TeacherGetDTO> GetByIdAsync(int id);
        Task<IEnumerable<TeacherGetDTO>> GetAllAsync();
        Task<TeacherCreateDTO> UpdateAsync(int id, TeacherCreateDTO dto);
        Task<TeacherCreateDTO> DeleteAsync(int id);
        Task<TeacherPatchDTO> PatchAsync(int id, TeacherPatchDTO dto);
    }
}
