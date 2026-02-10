using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.StudentDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentCreateDTO> CreateAsync(StudentCreateDTO dto);
        Task<StudentGetDTO> GetByIdAsync(int id);
        Task<IEnumerable<StudentGetDTO>> GetAllAsync();
        Task<StudentCreateDTO> UpdateAsync(int id, StudentCreateDTO dto);
        Task<StudentCreateDTO> DeleteAsync(int id);
        Task<StudentPatchDTO> PatchAsync(int id, StudentPatchDTO dto);
    }
}
