using System;
using System.Security.Cryptography;
using System.Text;

namespace Win.Common
{
    public class DEncryptHelper
    {
        public static string MD5Encrypt16(string input, string encoding)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.GetEncoding(encoding).GetBytes(input)), 4, 8);
            return t2.Replace("-", "").ToLower();
        }
        public static string MD5Encrypt32(string input, string encoding)
        {
            string cl = input;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
                                   // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.GetEncoding(encoding).GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + i.ToString("X");

            }
            return pwd.ToLower();
        }

    }
}