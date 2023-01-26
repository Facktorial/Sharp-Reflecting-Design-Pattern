using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    public enum EventType
    {
        None = 0,
        Fun = 1,
    }
    public class Event
    {
        public string Name { get; set; }
        public DateTime DateTime { get; set; }

        public EventType Type { get; set; }

        public Event()
        {
            Name = string.Empty;
            DateTime = DateTime.MinValue;
            Type = EventType.None;
        }
    }
}
