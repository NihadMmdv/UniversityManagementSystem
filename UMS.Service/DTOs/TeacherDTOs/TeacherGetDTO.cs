using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;

namespace UMS.Service.DTOs.TeacherDTOs
{
    public class TeacherGetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhotoUrl { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public List<int> LessonIds { get; set; }
        public int Salary { get; set; }
    }
}
