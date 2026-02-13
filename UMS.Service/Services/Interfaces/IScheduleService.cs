using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.ScheduleDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<ScheduleCreateDTO> CreateAsync(ScheduleCreateDTO dto);
        Task<ScheduleGetDTO> GetByIdAsync(int id);
        Task<IEnumerable<ScheduleGetDTO>> GetAllAsync();
        Task<ScheduleCreateDTO> UpdateAsync(int id, ScheduleCreateDTO dto);
        Task<ScheduleCreateDTO> DeleteAsync(int id);
    }
}