using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Service.DTOs.ScheduleDTOs
{
    public class ScheduleEntryDTO
    {
        public string LessonName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public List<string> TeacherNames { get; set; } = new();
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}
