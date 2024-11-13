using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DbTool.Bean;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;

namespace DbTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DbSet DbSet
        {
            get
            {
                string configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbTool.config");
                var text = FileHelper.Read(configurationPath);
                var crypto = new AESCrypto("awerfdgg");
                var b = crypto.DecryptStr(text);
                var dbSet = System.Text.Json.JsonSerializer.Deserialize<DbSet>(b);
                return dbSet;
            }
        }

        public App()
        {
            
        }
    }
}
