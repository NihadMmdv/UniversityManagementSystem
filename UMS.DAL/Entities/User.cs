using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Student"; // "Admin", "Student", "Teacher"
        public string? Name { get; set; }
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
