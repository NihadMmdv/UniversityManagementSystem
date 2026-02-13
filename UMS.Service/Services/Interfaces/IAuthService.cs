using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.AuthDTOs;
using UMS.Service.DTOs.ExamDTOs;
using UMS.Service.DTOs.LessonDTOs;
using UMS.Service.DTOs.ScheduleDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto);
        Task<ProfileDTO?> GetProfileAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
        Task<List<LessonGetDTO>> GetMyLessonsAsync(int userId);
        Task<List<MyExamDTO>> GetMyExamsAsync(int userId);
        Task<List<ScheduleEntryDTO>> GetMyScheduleAsync(int userId);
    }
}
