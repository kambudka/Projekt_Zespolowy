using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SystemRezerwacjiKortow.Models
{
    public class Currencies
    {
        public string table { get; set; }
        public string no { get; set; }
        public string effectiveDate { get; set; }
        public List<Currency> rates { get; set; }
    }
}