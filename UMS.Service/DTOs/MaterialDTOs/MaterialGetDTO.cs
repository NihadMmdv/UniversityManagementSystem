using System;

namespace UMS.Service.DTOs.MaterialDTOs
{
    public class MaterialGetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int LessonId { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }
}