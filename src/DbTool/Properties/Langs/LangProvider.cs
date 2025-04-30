using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace DbTool.Properties.Langs;

public class LangProvider: INotifyPropertyChanged
{
    private static readonly Lazy<LangProvider> _instance = new Lazy<LangProvider>(() => new LangProvider());
    public static LangProvider Instance => _instance.Value;

    private static string CultureInfoStr;

    public CultureInfo Culture
    {
        get => Lang.Culture;
        set
        {
            if (value == null) return;
            if (Equals(CultureInfoStr, value.EnglishName)) return;
            Lang.Culture = value;
            CultureInfoStr = value.EnglishName;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null)); // 通知所有属性更新
        }
    }

    public string GetLang(string key)
    {
        return Lang.ResourceManager.GetString(key, Culture);
    }

    public void SetLang(DependencyObject dependencyObject, DependencyProperty dependencyProperty, string key)
    {
        BindingOperations.SetBinding(dependencyObject, dependencyProperty, new Binding(key)
        {
            Source = Instance,
            Mode = BindingMode.OneWay
        });
    }

    public string this[string key]
    {
        get => Lang.ResourceManager.GetString(key, Culture) ?? $"[{key}]";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}