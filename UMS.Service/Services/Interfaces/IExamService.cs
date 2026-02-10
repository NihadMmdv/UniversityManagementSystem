using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.ExamDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface IExamService
    {
        Task<ExamCreateDTO> CreateAsync(ExamCreateDTO dto);
        Task<ExamGetDTO> GetByIdAsync(int id);
        Task<IEnumerable<ExamGetDTO>> GetAllAsync();
        Task<ExamCreateDTO> UpdateAsync(int id, ExamCreateDTO dto);
        Task<ExamCreateDTO> DeleteAsync(int id);
        Task<ExamPatchDTO> PatchAsync(int id, ExamPatchDTO dto);
    }
}
