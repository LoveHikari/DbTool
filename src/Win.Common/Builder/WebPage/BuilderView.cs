using System;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder.WebPage
{
    public class BuilderView
    {
        private readonly List<ColumnModel> _fieldlist;  //选择要生成的字段集合

        public BuilderView(List<ColumnModel> fieldlist)
        {
            _fieldlist = fieldlist;
        }
        public string GetAdd()
        {
            //读取模板
            string tempPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "App_Data\\WebTemplate\\Add.cmt");
            //获得循环部分
            string temp = System.FileHelper.ReadFile(tempPath);
            string item = temp.GetSubString("<t:ItemTemplate>", "</t:ItemTemplate>");

            StringBuilder sb = new StringBuilder();
            foreach (ColumnModel model in _fieldlist)
            {
                sb.Append(item.Replace("<#=Desc#>", string.IsNullOrWhiteSpace(model.Description)? model.ColumnName: model.Description).Replace("<#=ColumnName#>", model.ColumnName).ToFirstLower());
            }

            return temp.Replace($"<t:ItemTemplate>{item}</t:ItemTemplate>", sb.ToString());
        }
    }
}