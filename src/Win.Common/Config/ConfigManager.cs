using System;
using System.Data.Entity;
using System.IO;
using System.Threading;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;
using Win.Common.Config.Model;

namespace Win.Common.Config;
/// <summary>
/// 配置操作类
/// </summary>
public class ConfigManager
{
    private static readonly Lazy<ConfigManager> lazyInstance = new(() => new ConfigManager());
    public static ConfigManager Instance => lazyInstance.Value;

    private ConfigParm _configParm;
    private static readonly Lock _locker = new();

    public ConfigParm ConfigParm
    {
        get
        {
            if (_configParm == null)
                lock (_locker)
                    if (_configParm == null)
                    {
                        string configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbTool.config");
                        var text = FileHelper.Read(configurationPath);
                        var crypto = new AESCrypto("awerfdgg");
                        var b = crypto.DecryptStr(text);
                        _configParm = System.Text.Json.JsonSerializer.Deserialize<ConfigParm>(b);
                    }

            return _configParm;
        }
    }
    private ConfigManager()
    {
        string configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbTool.config");
        var text = FileHelper.Read(configurationPath);
        var crypto = new AESCrypto("awerfdgg");
        var b = crypto.DecryptStr(text);
        var dbSet = System.Text.Json.JsonSerializer.Deserialize<ConfigParm>(b);
    }

    public void UpdateConfig(ConfigParm cp)
    {
        _configParm = cp;
        var s = System.Text.Json.JsonSerializer.Serialize(cp);
        var crypto = new AESCrypto("awerfdgg");
        var b = crypto.EncryptBase64(s);
        FileHelper.Write(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbTool.config"), b);
    }
}