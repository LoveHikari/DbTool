using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Win.Models;

/******************************************************************************************************************
 * 
 * 
 * 说　明： 类型转换(版本：Version1.0.0)
 * 作　者：YuXiaoWei
 * 日　期：2016/08/29
 * 修　改：
 * 参　考：http://www.csframework.com/archive/2/arc-2-20110317-1144.htm
 * 备　注：暂无...
 * 
 * 
 * ***************************************************************************************************************/
namespace Win.Common
{
    public class CodeCommon
    {
        /// <summary>
        /// 获得主键或标识列列表
        /// </summary>
        /// <param name="fieldlist"></param>
        public static List<ColumnModel> GetPrimaryKeyList(List<ColumnModel> fieldlist)
        {
            List<ColumnModel> keyList = new List<ColumnModel>();
            foreach (ColumnModel field in fieldlist)
            {
                if (field.IsPrimaryKey || field.IsIdentity)
                {
                    keyList.Add(field);
                }
            }
            return keyList;
        }

        /// <summary>
        /// 主键列表中是否有标识列
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool IsHasIdentity(List<ColumnModel> keys)
        {
            bool flag = false;
            if (keys.Count > 0)
            {
                foreach (ColumnModel key in keys)
                {
                    if (key.IsIdentity)
                        flag = true;
                }
            }
            return flag;
        }
        /// <summary>
        /// 将SQLServer数据类型（如：varchar）转换为.Net类型（如：String）
        /// </summary>
        /// <param name="sqlTypeString">SQLServer数据类型</param>
        /// <returns></returns>
        public static string DbTypeToCS(string sqlTypeString)
        {
            string[] SqlTypeNames = new string[] { "int", "varchar","bit" ,"datetime","decimal","float","image","money",
                "ntext","nvarchar","smalldatetime","smallint","text","bigint","binary","char","nchar","numeric",
                "real","smallmoney", "sql_variant","timestamp","tinyint","uniqueidentifier","varbinary"};

            string[] DotNetTypes = new string[] {"int", "string","bool" ,"DateTime","Decimal","Double","Byte[]","Single",
                "string","string","DateTime","Int16","string","Int64","Byte[]","string","string","Decimal",
                "Single","Single", "Object","Byte[]","Byte","Guid","Byte[]"};

            int i = Array.IndexOf(SqlTypeNames, sqlTypeString.ToLower());

            return DotNetTypes[i];
        }

        public static string SqlTypeToDbType(string fieldTypeName)
        {
            throw new NotImplementedException();
        }
    }
}