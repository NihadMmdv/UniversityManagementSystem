using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Service.DTOs.ScheduleDTOs
{
    public class ScheduleGetDTO
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int SectionId { get; set; }
        public List<int> TeacherIds { get; set; } = new();
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}