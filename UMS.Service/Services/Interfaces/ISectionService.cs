using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.SectionDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface ISectionService
    {
        Task<SectionCreateDTO> CreateAsync(SectionCreateDTO dto);
        Task<SectionGetDTO> GetByIdAsync(int id);
        Task<IEnumerable<SectionGetDTO>> GetAllAsync();
        Task<SectionCreateDTO> UpdateAsync(int id, SectionCreateDTO dto);
        Task<SectionCreateDTO> DeleteAsync(int id);
        Task<SectionPatchDTO> PatchAsync(int id, SectionPatchDTO dto);
    }
}
