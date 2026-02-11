using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Service.DTOs.AuthDTOs;

namespace UMS.Service.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto);
    }
}
