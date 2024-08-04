using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessClassLibrary.Models
{
    public class FileItem
    {
        public string? Name { get; set; }
        public string? Icon { get; set; }

        public string? Path { get; set; }
    }
}