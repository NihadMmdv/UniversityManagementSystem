using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class  Section : BaseEntity
    {
        public string Name { get; set; }
        public List<int> StudentIds { get; set; }
        public List<Student> Students { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
   
}
