using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;

namespace UMS.Service.DTOs.LessonDTOs
{
    public class LessonCreateDTO 
    {
        public string Name { get; set; }
        public List<int> SectionIds { get; set; }
        public List<int> TeacherIds { get; set; }
    }
}
