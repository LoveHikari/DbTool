using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DbTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 获取屏幕高度
            double screenHeight = SystemParameters.WorkArea.Height;
            double screenWidth = SystemParameters.WorkArea.Width;
            this.MaxHeight = screenHeight; // 限制最大高度
            this.MaxWidth = screenWidth;  // 限制最大宽度
            // 如果窗口高度超过屏幕高度，则调整为屏幕高度
            if (this.Height > screenHeight)
            {
                this.Height = screenHeight;
            }
            // 如果窗口宽度超过屏幕宽度，则调整为屏幕宽度
            if (this.Width > screenWidth)
            {
                this.Width = screenWidth;
            }

        }
    }
}
