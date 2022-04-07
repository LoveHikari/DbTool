using DbTool.Models;
using Hikari.Common;
using Hikari.Common.IO;
using Hikari.WPF.MVVM;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Win.Common;
using Win.Common.Builder;
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
            Model.ConnectionString = "Persist Security Info=False;User ID=sa;Password=Atkj89715326;Initial Catalog=ANTOINE_DATABASE;Server=192.168.1.140";
            Model.ModelPath = "Models";
            Model.ModelPrefix = "M";
            Model.ProviderNameList = new List<string>()
            {
                "Sql Server", "MySql"
            };
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
            var builder = new BuilderModel(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix);
            var b = builder.CreatModel();
            Model.CodeContent = b;
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
                    var builder = new BuilderModel(models, Model.ModelPath, Model.ModelPrefix, Model.ModelSuffix);
                    var b = builder.CreatModel();
                    string modelName = Model.ModelPrefix + tableName.ToPascalCase() + Model.ModelSuffix;
                    FileHelper.WriteAsync(System.IO.Path.Combine(path, modelName + ".cs"), b).GetAwaiter().GetResult();
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