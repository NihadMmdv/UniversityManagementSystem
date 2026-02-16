using System;
using System.ComponentModel.DataAnnotations;
using UMS.Service.Validators;

namespace UMS.Service.DTOs.ExamDTOs
{
    public class ExamCreateDTO
    {
        [Required]
        public int LessonId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [NotInFuture(ErrorMessage = "Exam date cannot be in the future.")]
        public DateOnly ExamDate { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
        public int Score { get; set; }
    }
}
