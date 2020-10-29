using System;
using System.Collections.Generic;
using System.Text;

namespace projektwand_backend.Models
{
    class Response
    {
        public string range { get; set; }
        public string majorDimension { get; set; }
        public List<List<string>> values { get; set; }
    }
}
