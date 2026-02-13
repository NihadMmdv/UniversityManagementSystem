using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Service.DTOs.ExamDTOs
{
    public class MyExamDTO
    {
        public int Id { get; set; }
        public string LessonName { get; set; } = string.Empty;
        public DateOnly ExamDate { get; set; }
        public int Score { get; set; }
    }
}
