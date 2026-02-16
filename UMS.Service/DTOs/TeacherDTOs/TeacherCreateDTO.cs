using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;
using UMS.Service.Validators;

namespace UMS.Service.DTOs.TeacherDTOs
{
    public class TeacherCreateDTO
    {
        [Required, MaxLength(25)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(25)]
        public string Surname { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }

        [NotInFuture(ErrorMessage = "Date of birth cannot be in the future.")]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public List<int> LessonIds { get; set; } = [];

        [Range(0, int.MaxValue, ErrorMessage = "Salary must be non-negative.")]
        public int Salary { get; set; }
    }
}
