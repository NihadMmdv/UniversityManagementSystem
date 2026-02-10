using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class Exam : BaseEntity
    {
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public DateOnly ExamDate { get; set; }
        public int Score { get; set; }
        public Student Student { get; set; }
        public Lesson Lesson { get; set; }
    }
}
