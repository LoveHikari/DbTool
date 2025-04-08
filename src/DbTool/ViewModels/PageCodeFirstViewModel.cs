using System.Text;
using CommunityToolkit.Mvvm.Input;
using DbTool.Bean;
using DbTool.Models;
using Hikari.Common.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using Hikari.Common;
using Win.Common;
using Win.DAL.BLL;

namespace DbTool.ViewModels
{
    public class PageCodeFirstViewModel
    {
        private IBaseBll _bll;
        public Models.PageCodeFirstModel Model { get; set; }


        public PageCodeFirstViewModel()
        {
            Model = new PageCodeFirstModel();
        }

        /// <summary>
        /// 选择Model文件命令
        /// </summary>
        public ICommand SelectModelFileCommand => new AsyncRelayCommand<object>(async delegate (object? obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Model文件(*.cs)|*.cs" // 可以根据需要修改文件过滤器
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Model.ModelFilePath = openFileDialog.FileName;
                // 解析文件
                string content = FileHelper.Read(Model.ModelFilePath);
                Model.TableStructure = await ParseModel(content);
                Model.SqlText = GenerateSql(Model.TableStructure);
            }
        });

        /// <summary>
        /// 解析model
        /// </summary>
        /// <param name="code"></param>
        private async Task<List<TableStructureTreeNode>> ParseModel(string code)
        {
            List<TableStructureTreeNode> tables = new List<TableStructureTreeNode>();
            // 解析类名
            MatchCollection classMatches = Regex.Matches(code, @"((?:\[.*?\]\s*)*)(public|private|protected|internal)?\s*(static|sealed|abstract)?\s*class\s+(\w+)");
            foreach (Match classMatch in classMatches)
            {
                var tableStructure = new TableStructureTreeNode();

                var attributeMatchs = Regex.Matches(classMatch.Groups[1].Value, @"\[.*?\]");
                var tableName = "";
                foreach (Match attributeMatch in attributeMatchs)
                {
                    var reg = Regex.Match(attributeMatch.Value, "\\[Table\\(\"(.*?)\"\\)\\]");
                    tableName = reg.Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(tableName))
                    {
                        tableName = classMatch.Groups[4].Value;
                    }
                }
                tableStructure.TableName = tableName;
                tableStructure.ColumnName = tableName;

                // 解析类注释
                string classComment = ExtractComment(code, classMatch.Index);
                if (string.IsNullOrWhiteSpace(classComment))
                {
                    classComment = tableStructure.TableName;
                }
                tableStructure.TableDescription = classComment;
                tableStructure.Description = classComment;

                // 解析类中的属性
                string classCode = ExtractClassCode(code, classMatch.Index, classMatch.Length);
                MatchCollection propertyMatches = Regex.Matches(classCode, @"((?:\[.*?\]\s*)*)(public|private|protected|internal)?\s*(static|readonly)?\s*(\w+)\s+(\w+)\s*{");
                foreach (Match propertyMatch in propertyMatches)
                {
                    var node = new TableStructureTreeNode();

                    var propertyAttributeMatchs = Regex.Matches(propertyMatch.Groups[1].Value, @"\[.*?\]");
                    string propertyType = "";
                    string typeName = "";
                    string propertyName = "";
                    int? propertyLength = null;
                    string propertyComment = "";
                    foreach (Match propertyAttributeMatch in propertyAttributeMatchs)
                    {
                        var reg = Regex.Match(propertyAttributeMatch.Value, "\\[.*?Key.*?\\]");
                        if (reg.Length > 0)
                        {
                            node.IsPrimaryKey = true;
                        }
                        reg = Regex.Match(propertyAttributeMatch.Value, "Column\\(\"(.+?)\"(?:.*?TypeName.+?\"(.+?)\")?");

                        typeName = reg.Groups[1].Value;
                        if (string.IsNullOrWhiteSpace(typeName))
                        {
                            propertyType = propertyMatch.Groups[4].Value;
                            typeName = CodeCommon.CSToDbType(propertyType.TrimEnd("?"));
                        }
                        propertyName = reg.Groups[2].Value;
                        if (string.IsNullOrWhiteSpace(propertyName))
                        {
                            propertyName = propertyMatch.Groups[5].Value;

                        }
                        reg = Regex.Match(propertyAttributeMatch.Value, ".*?Required.*?");
                        if (reg.Length > 0)
                        {
                            node.IsCanNull = false;
                        }
                        reg = Regex.Match(propertyAttributeMatch.Value, ".*?StringLength\\((\\d+)");
                        if (reg.Length > 0)
                        {
                            propertyLength = reg.Groups[1].Value.ToInt32();
                        }


                        reg = Regex.Match(propertyAttributeMatch.Value, ".*?Comment\\(\"(.+?)\"");
                        if (reg.Length > 0)
                        {
                            propertyComment = reg.Groups[1].Value;
                        }

                    }

                    if (propertyType.IndexOf("?") > 0)
                    {
                        node.IsCanNull = true;
                    }

                    node.ColumnName = propertyName;
                    node.TypeName = typeName;
                    node.Length = propertyLength ?? 0;
                    node.Description = propertyComment;

                    if (string.IsNullOrWhiteSpace(propertyComment))
                    {
                        // 解析属性注释
                        propertyComment = ExtractComment(code, code.IndexOf(propertyMatch.Value));
                        if (string.IsNullOrWhiteSpace(propertyComment))
                        {
                            propertyComment = node.ColumnName;
                        }
                        node.Description = propertyComment;
                    }


                    tableStructure.Children.Add(node);
                }
                tables.Add(tableStructure);
            }

            return tables;
        }

        private string GenerateSql(List<TableStructureTreeNode> tables)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var table in tables)
            {
                sb.AppendLine($"create table {table.TableName}");
                sb.AppendLine("(");
                foreach (var child in table.Children)
                {
                    string typeName = child.TypeName;
                    if (child.TypeName.Contains("varchar"))
                    {
                        var reg = Regex.Match(child.TypeName, "varchar\\((\\d+)\\)");
                        if (reg.Length == 0)
                        {
                            typeName = $"{child.TypeName}({child.Length})";
                        }
                    }
                    sb.AppendSpace(2, $"{child.ColumnName} {typeName}");
                    if (child.IsIdentity)
                    {
                        sb.Append(" auto_increment");
                    }

                    if (child.DefaultVal != null)
                    {
                        sb.Append($" default {child.DefaultVal}");
                    }
                    if (!child.IsPrimaryKey)
                    {
                        sb.Append(!child.IsCanNull ? " not null" : " null");
                    }

                    sb.Append($" comment '{child.Description}'");
                    if (child.IsPrimaryKey)
                    {
                        sb.Append(" primary key");
                    }
                    sb.AppendLine(",");
                }
                sb.Remove(sb.Length - 3, 1);
                sb.AppendLine(")");
                sb.AppendSpace(2, $"comment '{table.TableDescription}';");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 初始化树
        /// </summary>
        private void InitTreeView(string providerName)
        {

        }
        private string ExtractClassCode(string code, int startIndex, int classLength)
        {
            int openBraceCount = 0;
            int endIndex = startIndex + classLength;
            while (endIndex < code.Length)
            {
                if (code[endIndex] == '{')
                {
                    openBraceCount++;
                }
                else if (code[endIndex] == '}')
                {
                    openBraceCount--;
                    if (openBraceCount == 0)
                    {
                        break;
                    }
                }
                endIndex++;
            }
            return code.Substring(startIndex, endIndex - startIndex + 1);
        }
        /// <summary>
        /// 解析类注释
        /// </summary>
        /// <param name="code"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string ExtractComment(string code, int index)
        {
            string commentPattern = @"///\s*(.*)(\r?\n|$)";
            string comment = "";
            MatchCollection matches = Regex.Matches(code.Substring(0, index), commentPattern, RegexOptions.RightToLeft);
            foreach (Match match in matches)
            {
                comment += match.Groups[1].Value.Trim();
            }

            var reg = Regex.Match(comment, "</summary>(.*?)<summary>");
            var c = reg.Groups[1].Value;
            return c;
        }
    }
}