﻿using DbTool.Models;
using Hikari.Common;
using Hikari.Common.IO;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Hikari.Mvvm.Command;
using Win.Common.Builder;
using Win.Common.Config;
using Win.DAL.BLL;
using Win.Models;

namespace DbTool.ViewModels
{
    public class PageDbFirstViewModel
    {
        private IBaseBll _bll;
        public Models.PageDbFirstModel Model { get; set; }


        public PageDbFirstViewModel()
        {
            Model = new PageDbFirstModel();
            Model.ConnectionString = ConfigManager.Instance.ConfigParm.DefaultConnString;
            Model.ModelPath = "Models";
            Model.ModelPrefix = "M";
            Model.RepositoryPath = "Repository";
            Model.RepositorySuffix = "Repository";
            Model.ApplicationPath = "Service";
            Model.ApplicationSuffix = "Service";

            Model.CurrentConfig = "Model";

            Model.ProviderNameList = new List<string>()
            {
                "Sql Server", "MySql"
            };
            Model.ProviderName = ConfigManager.Instance.ConfigParm.DefaultDbType;
            Model.CodeLanguage = 0;
        }
        /// <summary>
        /// 连接命令
        /// </summary>
        public ICommand ConnectCommand
        {
            get
            {
                return new DelegateCommand<object>(delegate (object obj)
                {
                    string providerName = (obj as string)!;
                    InitTreeView(providerName);
                });
            }
        }

        public ICommand SelectionChangedCommand => new DelegateCommand<object>(delegate (object obj)
        {
            var items = obj as IList;
            if (items.Count != 0)
            {
                var row = items[0] as DataRowView;
                var tableName = row[0].ToString();  // 选择的表
                var dt = _bll.GetColumnTable(tableName);


                Model.TableFieldList = dt.DefaultView;
                Model.PrimaryKeyList = dt.AsEnumerable().Where(m => m.Field<string>("IsPrimaryKey") == "true").Select(m => m.Field<string>("ColumnName")).ToList();
            }
            
        });
        /// <summary>
        /// 生成代码命令
        /// </summary>
        public ICommand GenerateCodeCommand => new DelegateCommand<object>(delegate (object obj)
        {
            List<ColumnModel> models = Model.TableFieldList.Table.ToList<ColumnModel>();
            // var builder = new BuilderModelEfCore(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix);
            BuilderModel builder = Model.CodeLanguage switch
            {
                0 => new BuilderModelEfCore(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix, Model.RepositoryPath, Model.RepositoryPrefix, Model.RepositorySuffix, Model.ApplicationPath, Model.ApplicationPrefix, Model.ApplicationSuffix),
                1 => new BuilderModelJavaJpa(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix, Model.RepositoryPath, Model.RepositoryPrefix, Model.RepositorySuffix, Model.ApplicationPath, Model.ApplicationPrefix, Model.ApplicationSuffix),
                _ => new BuilderModelEfCore(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix, Model.RepositoryPath, Model.RepositoryPrefix, Model.RepositorySuffix, Model.ApplicationPath, Model.ApplicationPrefix, Model.ApplicationSuffix)
            };
            Model.CodeContent = Model.CurrentConfig switch
            {
                "Model" => builder.CreatModel(),
                "Repository" => builder.CreatRepository(),
                "Application" => builder.CreatApplication(),
                _ => builder.CreatModel()
            };
        });
        /// <summary>
        /// 生成全部代码命令
        /// </summary>
        public ICommand GenerateAllCodeCommand => new DelegateCommand<object>(delegate (object obj)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            var path = dialog.SelectedPath;
            ThreadPool.QueueUserWorkItem(state =>
            {
                foreach (DataRow row in Model.TableList.Table.Rows)
                {
                    var tableName = row[0].ToString();
                    var dt = _bll.GetColumnTable(tableName);
                    List<ColumnModel> models = dt.ToList<ColumnModel>();
                    //var builder = new BuilderModelEfCore(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix);
                    BuilderModel builder = Model.CodeLanguage switch
                    {
                        0 => new BuilderModelEfCore(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix, Model.RepositoryPath, Model.RepositoryPrefix, Model.RepositorySuffix, Model.ApplicationPath, Model.ApplicationPrefix, Model.ApplicationSuffix),
                        1 => new BuilderModelJavaJpa(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix, Model.RepositoryPath, Model.RepositoryPrefix, Model.RepositorySuffix, Model.ApplicationPath, Model.ApplicationPrefix, Model.ApplicationSuffix),
                        _ => new BuilderModelEfCore(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix, Model.RepositoryPath, Model.RepositoryPrefix, Model.RepositorySuffix, Model.ApplicationPath, Model.ApplicationPrefix, Model.ApplicationSuffix)
                    };
                    var b = builder.CreatModel();
                    string modelName = Model.ModelPrefix + tableName.ToPascalCase() + Model.ModelSuffix;
                    string pathName = Model.CodeLanguage switch
                    {
                        0 => System.IO.Path.Combine(path, modelName + ".cs"),
                        1 => System.IO.Path.Combine(path, modelName + ".java"),
                        _ => System.IO.Path.Combine(path, modelName + ".cs")
                    };
                    FileHelper.WriteAsync(pathName, b).GetAwaiter().GetResult();
                }

                MessageBox.Show("生成成功！");
            });

        });


        /// <summary>
        /// 初始化树
        /// </summary>
        private void InitTreeView(string providerName)
        {
            switch (providerName)
            {
                case "System.Data.SQLite":
                    //dt = Win.DAL.BLL.SQLiteBll.GetAllTable();
                    break;
                case "MySql":
                    _bll = new MySqlBll(Model.ConnectionString);
                    break;
                case "Sql Server":
                default:
                    _bll = new SqlServerBll(Model.ConnectionString);
                    break;
            }
            Model.TableList = _bll.GetAllTable().DefaultView;
        }
    }
}