using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.Service.DTOs.ExamDTOs
{
    public class ExamGetDTO
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int StudentId { get; set; }
        public DateOnly ExamDate { get; set; }
        public int Score { get; set; }
    }
}
