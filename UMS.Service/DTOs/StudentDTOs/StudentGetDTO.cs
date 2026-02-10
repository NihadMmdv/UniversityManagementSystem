using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;

namespace UMS.Service.DTOs.StudentDTOs
{
    public class StudentGetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int SectionId { get; set; }
        public string PhotoUrl { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateOnly EnrollmentDate { get; set; }

    }
}
