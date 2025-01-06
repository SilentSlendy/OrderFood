using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;


namespace POS.Helpers
{
    /// <summary>
    /// Mã hóa Feistel: Mã hóa một số nguyên thành một chuỗi Base62
    /// </summary>
    public class FeistelCipher
    {
        private int _rounds;  // Số vòng mã hóa
        private int[] _subkeys;  // Khóa con sinh từ khóa chính

        private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Khởi tạo một đối tượng mã hóa Feistel
        /// </summary>
        /// <param name="rounds"></param>
        /// <param name="key"></param>
        public FeistelCipher(int rounds, string key)
        {
            _rounds = rounds;
            _subkeys = GenerateSubkeys(key, rounds);
        }

        /// <summary>
        /// Sinh khóa con từ khóa chính
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rounds"></param>
        /// <returns></returns>
        private int[] GenerateSubkeys(string key, int rounds)
        {
            // Sinh khóa con từ khóa chính
            var subkeys = new int[rounds];
            using (var sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha256.ComputeHash(keyBytes);

                for (int i = 0; i < rounds; i++)
                {
                    subkeys[i] = BitConverter.ToInt32(hash, (i * 4) % hash.Length);
                }
            }
            return subkeys;
        }

        /// <summary>
        /// Hàm vòng F: dùng XOR với subkey + hash cơ bản
        /// </summary>
        /// <param name="right"></param>
        /// <param name="subkey"></param>
        /// <returns></returns>
        private int RoundFunction(int right, int subkey)
        {
            // Hàm vòng F: dùng XOR với subkey + hash cơ bản
            return (right ^ subkey) & 0xFFFF;
        }

        /// <summary>
        /// Mã hóa một số nguyên thành chuỗi Base62
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Encrypt(int value)
        {
            // Sinh salt ngẫu nhiên
            Random random = new Random();
            int salt = random.Next(0, 0xFFFF); // 16-bit ngẫu nhiên

            // Chia số nguyên thành hai phần: trái và phải
            int left = (value ^ salt) >> 16;   // Lấy 16 bit cao
            int right = (value ^ salt) & 0xFFFF;  // Lấy 16 bit thấp

            for (int i = 0; i < _rounds; i++)
            {
                int newLeft = right;
                int newRight = left ^ RoundFunction(right, _subkeys[i]);
                left = newLeft;
                right = newRight;
            }

            // Ghép lại thành một số nguyên 64 bit
            long cipherValue = ((long)left << 16) | (long)right;

            // Kết hợp salt và cipherValue thành một số duy nhất
            long combinedValue = (long)salt << 32 | cipherValue;

            // Encode thành chuỗi Base62
            return ToBase62(combinedValue);
        }

        /// <summary>
        /// Giải mã một chuỗi Base62 thành số nguyên
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public int Decrypt(string cipherText)
        {
            // Decode từ Base62
            long combinedValue = FromBase62(cipherText);

            // Tách salt và cipherValue
            int salt = (int)(combinedValue >> 32); // Lấy 32 bit cao làm salt
            long cipherValue = combinedValue & 0xFFFFFFFF; // Lấy 32 bit thấp làm giá trị mã hóa

            // Chia thành hai phần: trái và phải
            int left = (int)(cipherValue >> 16);
            int right = (int)(cipherValue & 0xFFFF);

            // Giải mã qua các vòng
            for (int i = _rounds - 1; i >= 0; i--)
            {
                int newRight = left;
                int newLeft = right ^ RoundFunction(left, _subkeys[i]);
                left = newLeft;
                right = newRight;
            }

            // Khôi phục giá trị ban đầu bằng cách XOR với salt
            return ((left << 16) | right) ^ salt;
        }

        /// <summary>
        /// Chuyển số nguyên thành chuỗi Base62
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ToBase62(long value)
        {
            // Chuyển số nguyên thành chuỗi Base62
            var result = new StringBuilder();
            do
            {
                int index = (int)(value % 62);
                result.Insert(0, Base62Chars[index]);
                value /= 62;
            } while (value > 0);

            return result.ToString();
        }

        /// <summary>
        /// Chuyển chuỗi Base62 thành số nguyên
        /// </summary>
        /// <param name="base62"></param>
        /// <returns></returns>
        private long FromBase62(string base62)
        {
            // Chuyển chuỗi Base62 thành số nguyên
            long result = 0;
            foreach (char c in base62)
            {
                result = result * 62 + Base62Chars.IndexOf(c);
            }
            return result;
        }
    }
}
