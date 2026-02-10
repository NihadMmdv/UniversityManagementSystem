using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMS.DAL.Entities
{
    public class Teacher: BasePerson
    {
        public int Salary { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}
