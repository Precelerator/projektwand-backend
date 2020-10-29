using System;
using System.Collections.Generic;
using System.Text;

namespace projektwand_backend.Models
{
    class Project
    {
        public string projektname { get; set; }
        public string kurzbeschreibung { get; set; }
        public string ausfuehrlicheBeschreibung { get; set; }
        public string kurzbeschreibungErsteller { get; set; }
        public string kategorie { get; set; }
        public string suchtNach { get; set; }
        public string onlineSeit { get; set; }
    }
}
