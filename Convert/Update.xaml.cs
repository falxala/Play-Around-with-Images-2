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
using System.Windows.Shapes;

namespace PlayAroundwithImages2
{
    /// <summary>
    /// Update.xaml の相互作用ロジック
    /// </summary>
    public partial class Update : Window
    {
        string[] upinfo;
        public Update(string[] info)
        {
            InitializeComponent();
            upinfo = info;
            text.Text = "UPDATE" + info[0]+"\r\n\r\n";
            text.Text += info[1];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(upinfo[2]);
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
