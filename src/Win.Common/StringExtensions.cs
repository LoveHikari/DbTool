using System;
using System.Text;

namespace Win.Common
{
    public static class StringExtensions
    {
        //public static string ToPascalCase(this string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //        return value;

        //    if (value.IndexOf('_') >= 0)
        //    {
        //        var parts = value.Split('_');
        //        var sb = new StringBuilder();
        //        foreach (var part in parts)
        //        {
        //            if (string.IsNullOrEmpty(part))
        //                continue;
        //            var str = part.ToCamelCase();
        //            sb.Append(char.ToUpper(str[0]) + str.Substring(1, str.Length - 1));
        //        }

        //        return sb.ToString();
        //    }

        //    var camelCase = value.ToCamelCase();
        //    return char.ToUpper(camelCase[0]) + camelCase.Substring(1, camelCase.Length - 1);
        //}

        public static string ToCamelCase(this string value)
        {
            int LowerCaseOffset = 'a' - 'A';
            if (string.IsNullOrEmpty(value))
                return value;

            var len = value.Length;
            var newValue = new char[len];
            var firstPart = true;

            for (var i = 0; i < len; ++i)
            {
                var c0 = value[i];
                var c1 = i < len - 1 ? value[i + 1] : 'A';
                var c0isUpper = c0 is >= 'A' and <= 'Z';
                var c1isUpper = c1 is >= 'A' and <= 'Z';

                if (firstPart && c0isUpper && (c1isUpper || i == 0))
                    c0 = (char)(c0 + LowerCaseOffset);
                else
                    firstPart = false;

                newValue[i] = c0;
            }

            return new string(newValue);
        }
    }
}