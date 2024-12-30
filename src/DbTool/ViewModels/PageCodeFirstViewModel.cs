using DbTool.Models;
using Hikari.Common;
using Hikari.Common.IO;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DbTool.Bean;
using Hikari.Mvvm.Command;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Win.Common.Builder;
using Win.Common.Config;
using Win.DAL.BLL;
using Win.Models;

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
            }
        });

        /// <summary>
        /// 解析model
        /// </summary>
        /// <param name="code"></param>
        private async Task<List<TableStructureTreeNode>> ParseModel(string code)
        {
            var tableStructure = new List<TableStructureTreeNode>();

            var tree = CSharpSyntaxTree.ParseText(code);
            var root = await tree.GetRootAsync();

            var classes = root.DescendantNodes()
                              .OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classes)
            {


                var properties = classDeclaration.DescendantNodes()
                                                  .OfType<PropertyDeclarationSyntax>();
                var children = new List<TableStructureTreeNode>();
                foreach (var property in properties)
                {
                    var propertyName = property.Identifier.Text;
                    var propertyType = property.Type.ToString();
                    var attributes = property.AttributeLists.SelectMany(a => a.Attributes)
                                                             .Select(attr => attr.ToString());

                    //Console.WriteLine($"  Property: {propertyName} ({propertyType})");
                    foreach (var attribute in attributes)
                    {
                        //Console.WriteLine($"    Attribute: {attribute}");
                    }
                    children.Add(new TableStructureTreeNode()
                    {
                        ColumnName = propertyName
                    });
                }
                tableStructure.Add(new TableStructureTreeNode()
                {
                    ColumnName = classDeclaration.Identifier.Text,
                    TableName = classDeclaration.Identifier.Text,
                    Children = children
                });
            }
            return tableStructure;
        }


        /// <summary>
        /// 初始化树
        /// </summary>
        private void InitTreeView(string providerName)
        {

        }
    }
}