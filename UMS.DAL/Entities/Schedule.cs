using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class Schedule : BaseEntity
    {
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;

        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public List<Teacher> Teachers { get; set; } = new();

        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
