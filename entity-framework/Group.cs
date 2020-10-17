using System;
using System.Collections.Generic;
using System.Text;

namespace entity_framework
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Faculty Faculty { get; set; }
        public List<Student> Students { get; set; }

        public Group()
        {
            Students = new List<Student>();
        }
    }
}
