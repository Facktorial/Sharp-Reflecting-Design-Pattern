using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    public class Organizer : Person
    {
        Event EventOrganize { get; set; } 
        public Organizer() : base()
        {
            EventOrganize = new Event();
        }

        public Organizer(string name, int age, Event ev)
        {
            Name = name;
            Age = age;
            EventOrganize = ev;
            SALARY_CONSTANT = 2000;
        }
    }
}
