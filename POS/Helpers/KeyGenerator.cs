using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Helpers
{
    using System.Security.Cryptography;
    public static class KeyGenerator
    {
        public static byte[] GenerateRandomKey(int size)
        {
            byte[] key = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }
    }
}
