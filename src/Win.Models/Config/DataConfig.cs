using System;
using Hikari.Common;

namespace Win.Models.Config
{
    public class DataConfig
    {
        private readonly string _dbPath;
        private readonly string _nsPath;

        #region instance
        private static volatile DataConfig _instance = null;
        private static readonly object lockHelper = new object();

        private DataConfig()
        {
            _dbPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "App_Data\\Config\\dataBaseConfig.xml");
            _nsPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "App_Data\\Config\\namespaceConfig.xml");
        }
        public static DataConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new DataConfig();
                    }
                }
                return _instance;
            }
        }
        #endregion
        
        public DataBaseConfig GetDataBaseConfig()
        {
            if (!System.IO.File.Exists(_dbPath))
            {
                var db = new DataBaseConfig()
                {
                    Id = 1,
                    ConnStr = "",
                    DatabaseName = "",
                    ProviderName = "",
                    ServerName = "",
                    Pwd = "",
                    Uid = ""
                };
                XmlHelper.XmlSerilizeToFile(db, _dbPath);
            }
            return XmlHelper.XmlDeserialize<Win.Models.DataBaseConfig>(_dbPath);
        }

        public void SetDataBaseConfig(DataBaseConfig db)
        {
            XmlHelper.XmlSerilizeToFile(db, _dbPath);
        }

        public NamespaceConfig GetNamespaceConfig()
        {
            if (!System.IO.File.Exists(_nsPath))
            {
                var db = new NamespaceConfig()
                {
                    Id = 1,
                    BllPath = "",
                    BllSuffix = "",
                    DalPath = "",
                    DalSuffix = "",
                    ModelPath = "",
                    ModelSuffix = ""
                };
                XmlHelper.XmlSerilizeToFile(db, _nsPath);
            }
            return XmlHelper.XmlDeserialize<Win.Models.NamespaceConfig>(_nsPath);
        }
        public void SetNamespaceConfig(NamespaceConfig db)
        {
            XmlHelper.XmlSerilizeToFile(db, _nsPath);
        }
    }
}