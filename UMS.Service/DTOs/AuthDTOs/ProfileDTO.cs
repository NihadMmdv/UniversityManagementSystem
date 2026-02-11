using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Service.DTOs.AuthDTOs
{
    public class ProfileDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        // Student only
        public string? ClassName { get; set; }
    }
}
