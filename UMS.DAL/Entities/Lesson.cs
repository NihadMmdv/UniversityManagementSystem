using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class Lesson:BaseEntity
    {
        public string Name { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Section> Sections { get; set; }
    }
}
