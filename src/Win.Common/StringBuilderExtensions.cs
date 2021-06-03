using System;
using System.Text;

namespace Win.Common
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 删除最后一个字符
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="strchar">要删除的字符</param>
        /// <param name="sc">大小写排序规则</param>
        /// <returns></returns>
        public static StringBuilder DelLastChar(this StringBuilder sb, string strchar, StringComparison sc = StringComparison.CurrentCulture)
        {
            string str = sb.ToString();
            int length = str.LastIndexOf(strchar,sc);
            if (length <= 0)
                return sb;
            sb.Clear().Append(str.Substring(0, length));
            return sb;
        }
    }
}