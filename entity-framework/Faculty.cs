using System;
using System.Collections.Generic;
using System.Text;

namespace entity_framework
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Group> Groups { get; set; }

        public Faculty()
        {
            Groups = new List<Group>();
        }
    }
}
