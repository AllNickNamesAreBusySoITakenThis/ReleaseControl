using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace ReleaseControlLib
{
    public class HashService
    {
        public static string ComputeMD5Checksum(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = System.IO.File.OpenRead(path))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    byte[] checkSum = md5.ComputeHash(fileData);
                    string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                    return result;
                }
            }
            else
            {
                return "";
            }
        }
    }
}
