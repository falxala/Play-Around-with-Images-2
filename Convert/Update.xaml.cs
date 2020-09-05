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
using System.Reflection;
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
            var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            var comObj = axIWebBrowser2.GetValue(browser, null);
            comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new object[] { true });

            upinfo = info;
            title.Text += upinfo[0];
            var a = "<html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'><head></head><body>" + Markdig.Markdown.ToHtml(upinfo[1]) + " </ body ></ html>";
            browser.NavigateToString(a);
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
