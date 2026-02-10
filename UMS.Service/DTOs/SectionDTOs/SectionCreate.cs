using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.DAL.Entities;

namespace UMS.Service.DTOs.SectionDTOs
{
    public class SectionCreateDTO
    {
        public string Name { get; set; }
        public List<int> StudentIds { get; set; }
        public List<int> LessonIds { get; set; }
    }
}
