using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Helpers
{
    static class KeyHandler
    {
        public static void SaveKeyToFile(byte[] key, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, key);
                Debug.WriteLine("Key saved to file successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while saving the key: " + ex.Message);
            }
        }

        public static byte[] ReadKeyFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] key = File.ReadAllBytes(filePath);
                    Debug.WriteLine("Key read from file successfully.");
                    return key;
                }
                else
                {
                    Debug.WriteLine("File does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while reading the key: " + ex.Message);
                return null;
            }
        }
    }
}

