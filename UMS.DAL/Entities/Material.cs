using System;

namespace UMS.DAL.Entities
{
    public enum MaterialType
    {
        Material = 0,
        Question = 1
    }

    public class Material : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public MaterialType Type { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}