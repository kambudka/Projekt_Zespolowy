using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SystemRezerwacjiKortow.Models
{
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string color { get; set; }
        public string payment { get; set; }
        public string name { get; set; }
        public string organizer { get; set; }
        public string description { get; set; }
        public int length { get; set; }
        public int type { get; set; }


    }
}