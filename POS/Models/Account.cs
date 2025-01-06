using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Account
    {
        public byte[] Username { get; set; }
        public byte[] Username_iv { get; set; }
        public byte[] Password { get; set; }
        public byte[] Password_iv { get; set; }
        public string UsernameString { get; set; }
        public string PasswordString { get; set; }
    }
}
