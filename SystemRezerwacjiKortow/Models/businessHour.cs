using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SystemRezerwacjiKortow.Models
{
    public class businessHour
    {
        public List<int> dow = new List<int>();
        public string startTime { get; set; }
        public string endTime { get; set; }
    }
}