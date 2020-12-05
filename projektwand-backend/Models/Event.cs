using System;
using System.Collections.Generic;
using System.Text;

namespace projektwand_backend.Models
{
    class Event
    {
        public string datum { get; set; }
        public string uhrzeit { get; set; }
        public string titel { get; set; }
        public string beschreibung { get; set; }
        public string zoomLink { get; set; }
    }
}
