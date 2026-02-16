using System;
using System.ComponentModel.DataAnnotations;
using UMS.Service.Validators;

namespace UMS.Service.DTOs.StudentDTOs
{
    public class StudentCreateDTO
    {
        [Required, MaxLength(25)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(25)]
        public string Surname { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        public int? SectionId { get; set; }

        public string? PhotoUrl { get; set; }

        [NotInFuture(ErrorMessage = "Date of birth cannot be in the future.")]
        public DateOnly DateOfBirth { get; set; }

        public DateOnly EnrollmentDate { get; set; }
    }
}
