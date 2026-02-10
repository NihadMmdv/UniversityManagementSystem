using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class Student : BasePerson
    {
        public int SectionId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public List<int> ExamIds { get; set; } = new();
        public Section Section { get; set; }
        public List<Exam> Exams { get; set; } = new();
    }
}
