using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessClassLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string username { get; set; }
        public required string password { get; set; }
        public required string email { get; set; }
    }
}
