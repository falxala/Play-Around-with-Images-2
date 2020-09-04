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
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Threading;
using System.Net;
using System.IO;
using System.Net.Http;
// Windows API Code Pack のダイアログの名前空間を using
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace PlayAroundwithImages2
{
    /// <summary>
    /// SubWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SubWindow : Window
    {
        public SubWindow()
        {
            InitializeComponent();
        }

        public class ItemSet
        {
            // DisplayMemberとValueMemberにはプロパティで指定する
            public String ItemDisp { get; set; }
            public object ItemValue { get; set; }

            // プロパティをコンストラクタでセット
            public ItemSet(int v , String s)
            {
                ItemDisp = s;
                ItemValue = v;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //よく使う拡張子だけを抜き出す(enum参照)
            var extLsit = new[] { 6, 17, 53, 74, 90, 105, 171, 185, 221, 239 };

            //OpenMPバージョン
            //var extLsit = new[] { 6, 15, 50, 72, 88, 103, 169, 183, 219, 237 };

            //コンボボックスに拡張子名と値をセット
            foreach (var Value in Enum.GetValues(typeof(ImageMagick.MagickFormat)))
            {
                foreach (int index in extLsit)
                {
                    if ((int)Value == index)
                    {
                        string name = Enum.GetName(typeof(ImageMagick.MagickFormat), Value);
                        ComboBox_extension.Items.Add(new ItemSet((int)Value, name));
                    }
                }
            }
            ComboBox_extension.DisplayMemberPath = "ItemDisp";
            ComboBox_extension.SelectedIndex = 5;
            if (Mainwin.subDockFlag == false)
                Dock_checkbox.IsChecked = true;

            selectDirTextBox.Text = Sub_CnvOption.SaveDirectory;

            Type t = Sub_CnvOption.GetType();


            //Info_TextBox.Text = Sub_CnvOption.Transform.ToString();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        private Process_Image.ConvertOptions sendData;
        private Process_Image.ConvertOptions Sub_CnvOption = new Process_Image.ConvertOptions();
        public MainWindow Mainwin;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Select_folder.IsEnabled = false;
            // ダイアログのインスタンスを生成
            var dialog = new CommonOpenFileDialog("フォルダーの選択");

            // 選択形式をフォルダースタイルにする IsFolderPicker プロパティを設定
            dialog.IsFolderPicker = true;

            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // ダイアログを表示
            if (dialog.ShowDialog(Mainwin) == CommonFileDialogResult.Ok)
            {
                selectDirTextBox.Text = dialog.FileName;
            }
            Select_folder.IsEnabled = true;
        }
        public Process_Image.ConvertOptions SendData
        {
            set
            {
                Sub_CnvOption = value;

                sendData = value;
                Width_TextBox.Text = sendData.Size.Width.ToString();
                Height_TextBox.Text = sendData.Size.Height.ToString();
            }
            get
            {
                return sendData;
            }
        }

        private void ComboBox_extension_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImageMagick.MagickFormat magickFormat = (ImageMagick.MagickFormat)((ItemSet)ComboBox_extension.SelectedItem).ItemValue;
            Sub_CnvOption.Format = magickFormat;
            SetMainCnvOption();
        }

        /// <summary>
        /// 変換時にメモリが不足しないか事前に計算
        /// </summary>
        public void CheckMemory(out long deficiencyMemory, out bool checkValue, System.Collections.IList Imagelist, int Parallelism, System.Drawing.Size convetedSize)
        {
            checkValue = true;

            var selected = new List<long>();
            foreach (Model.drop_Image list in Imagelist)
            {
                long product = list.Image_size.Width * list.Image_size.Height;
                selected.Add(product);
            }
            selected.Sort((a, b) => (int)b - (int)a);

            long product_all = 0;
            int count = 0;
            foreach (var i in selected)
            {
                if (count < Parallelism)
                    product_all += i;
                else
                    continue;
                count++;
            }

            long TotalVisibleMemorySize = 0;
            long FreePhysicalMemory = 0;

            System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_OperatingSystem");
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                TotalVisibleMemorySize = long.Parse(mo["TotalVisibleMemorySize"].ToString());
                FreePhysicalMemory = long.Parse(mo["FreePhysicalMemory"].ToString()) / 1024;
                //合計物理メモリ
                Console.WriteLine("合計物理メモリ:{0}KB", mo["TotalVisibleMemorySize"]);
                //利用可能な物理メモリ
                Console.WriteLine("利用可能物理メモリ:{0}KB", mo["FreePhysicalMemory"]);
                mo.Dispose();
            }
            moc.Dispose();
            mc.Dispose();
            //推定メモリ(MB)
            long Estimatedmemory = 0;
            if (Sub_CnvOption.Transform == true && Sub_CnvOption.LiquidRescale ==false)
                Estimatedmemory = (product_all * 2 * 4 / 1024 / 1024 + ((long)convetedSize.Width * (long)convetedSize.Height * 2 * 4 / 1024 / 1024) * 2 * (long)count);
            else if (Sub_CnvOption.LiquidRescale)
                //LiquidRescale有効時のメモリ使用量は不明　推測
                Estimatedmemory = (product_all * 2 * 4 / 1024 / 1024 + ((long)convetedSize.Width * (long)convetedSize.Height * 2 * 4 / 1024 / 1024) * 4 * (long)count);
            else
                Estimatedmemory = product_all * 2 * 4 / 1024 / 1024 * 2 +((long)convetedSize.Width * (long)convetedSize.Height * 2 * 4 / 1024 / 1024) * 2 * (long)count;
            //Q16の場合キャッシュは2バイトなので*2
            Info_TextBox.Text = "最大使用メモリ : " + Estimatedmemory + "MB";
            Info_TextBox.Text += "\r\n FreeMemory : " + FreePhysicalMemory;
            Info_TextBox.Text += " / " + TotalVisibleMemorySize / 1024 + " [MB]";

            deficiencyMemory = Estimatedmemory - FreePhysicalMemory;

            //物理メモリが推定メモリを超過した場合falseを返す
            if (FreePhysicalMemory< Estimatedmemory)
            {
                checkValue = false;
            }
        }

        /// <summary>
        /// サブウィンドウの設定をメインに反映
        /// </summary>
        private void SetMainCnvOption()
        {
            try
            {
                Mainwin.ReceiveData = Sub_CnvOption;
            }
            catch
            {

            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(Dock_checkbox.IsChecked == true)
            {
                Mainwin.subDockFlag = true;
            }
            else
            {
                Mainwin.subDockFlag = false;
            }
        }

        bool Chainflag = false;
        private void ChainButton_Click(object sender, RoutedEventArgs e)
        {
            var img = new System.Windows.Controls.Image();
            if (Chainflag == false)
            {
                img.Source = new BitmapImage(new Uri("pack://application:,,,/PlayAroundwithImages2;component/Resources/Chain.png"));
                Chainflag = true;
            }
            else
            {
                img.Source = new BitmapImage(new Uri("pack://application:,,,/PlayAroundwithImages2;component/Resources/Unchain.png"));
                Chainflag = false;
            }

            ChainButton.Content = img;

        }

        /// <summary>
        /// テキストボックスの値をoptionにセット
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void SetSizeOptionFromText()
        {
            try
            {
                Sub_CnvOption.Size = new System.Drawing.Size(int.Parse(Width_TextBox.Text), int.Parse(Height_TextBox.Text));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Width_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Chainflag)
                Height_TextBox.Text = Width_TextBox.Text;
            SetSizeOptionFromText();
            SetMainCnvOption();
        }

        private void Height_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Chainflag)
                Width_TextBox.Text = Height_TextBox.Text;
            SetSizeOptionFromText();
            SetMainCnvOption();
        }

        private void Width_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);

            if (e.Text == "+")
            {
                //構造体のため一旦コピーしてから値を変更して戻す
                var Temp = Sub_CnvOption.Size;
                Temp.Width++;
                Sub_CnvOption.Size = Temp;

                Width_TextBox.Text = Sub_CnvOption.Size.Width.ToString();
                return;
            }
            if (e.Text == "-")
            {
                var Temp = Sub_CnvOption.Size;
                Temp.Width--;
                Sub_CnvOption.Size = Temp;
                Width_TextBox.Text = Sub_CnvOption.Size.Width.ToString();
                return;
            }
        }

        private void Height_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);

            if (e.Text == "+")
            {
                var Temp = Sub_CnvOption.Size;
                Temp.Height++;
                Sub_CnvOption.Size = Temp;

                Height_TextBox.Text = Sub_CnvOption.Size.Height.ToString();
                return;
            }
            if (e.Text == "-")
            {
                var Temp = Sub_CnvOption.Size;
                Temp.Height--;
                Sub_CnvOption.Size = Temp;

                Height_TextBox.Text = Sub_CnvOption.Size.Height.ToString();
                return;
            }
        }

        private void tf_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            Sub_CnvOption.Transform = true;
            Width_TextBox.IsEnabled = true;
            Height_TextBox.IsEnabled = true;
            Mainwin.limit_longside_tb.IsEnabled = false;
            try
            {
                LiquidRescale_toggle.IsEnabled = true;
                var C = ColorTranslator.FromHtml("#FF319C3F");
                LiquidRescale_toggle.TrackBackgroundOnColor = System.Windows.Media.Color.FromArgb(C.A, C.R, C.G, C.B);
                LiquidRescale_toggle.IsOn = !LiquidRescale_toggle.IsOn;
                LiquidRescale_toggle.IsOn = !LiquidRescale_toggle.IsOn;
            }
            catch { }
            SetMainCnvOption();
        }

        private void tf_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Sub_CnvOption.Transform = false;
            Width_TextBox.IsEnabled = false;
            Height_TextBox.IsEnabled = false;
            Mainwin.limit_longside_tb.IsEnabled = true;
            try
            {
                LiquidRescale_toggle.IsEnabled = false;
                var C = ColorTranslator.FromHtml("#FF88A88C");
                LiquidRescale_toggle.TrackBackgroundOnColor = System.Windows.Media.Color.FromArgb(C.A, C.R, C.G, C.B);
                LiquidRescale_toggle.IsOn = !LiquidRescale_toggle.IsOn;
                LiquidRescale_toggle.IsOn = !LiquidRescale_toggle.IsOn;
            }
            catch { }
            SetMainCnvOption();
        }

        private void ow_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (ow_checkbox.IsChecked == true)
            {
                Sub_CnvOption.Overwrite = true;
            }
            else
            {
                Sub_CnvOption.Overwrite = false;
            }
            SetMainCnvOption();
        }

        private void selectDirTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sub_CnvOption.SaveDirectory = selectDirTextBox.Text;
            SetMainCnvOption();
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", Sub_CnvOption.SaveDirectory);
        }

        private void LiquidRescale_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.LiquidRescale = !LiquidRescale_toggle.IsOn;
            SetMainCnvOption();
        }

        //冗長な書き方　後でリファクタリング
        private void Rotate_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
                if (Math.Abs(int.Parse(Rotate_TextBox.Text)) <= 180)
                {
                    if (e.Text == "+")
                    {
                        var Temp = Sub_CnvOption.Rotate;
                        if (Temp < 180)
                            Temp++;
                        Sub_CnvOption.Rotate = Temp;

                        Rotate_TextBox.Text = Sub_CnvOption.Rotate.ToString();
                        return;
                    }
                    if (e.Text == "-")
                    {
                        var Temp = Sub_CnvOption.Rotate;
                        if (Temp > -180)
                            Temp--;
                        Sub_CnvOption.Rotate = Temp;

                        Rotate_TextBox.Text = Sub_CnvOption.Rotate.ToString();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Rotate_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Sub_CnvOption.Rotate = int.Parse(Rotate_TextBox.Text);
                Rotate_Slider.Value = Sub_CnvOption.Rotate;
            }
            catch
            {
                Sub_CnvOption.Rotate = 0;
            }

            SetMainCnvOption();
        }

        private void Rotate_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Sub_CnvOption.Rotate = (int)Rotate_Slider.Value;
            Rotate_TextBox.Text = Sub_CnvOption.Rotate.ToString();
        }

        private void Mirror_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.Mirror = !Mirror_toggle.IsOn;
            SetMainCnvOption();
        }

        private async void Rotate_Slider_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await Task.Delay(50);
            Rotate_Slider.Value = 0;
        }

        private void Rotate_Slider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Rotate_Slider.Value++;
            else if (e.Delta < 0)
                Rotate_Slider.Value--;
        }

        private void passthrough_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.Passthrough = !passthrough_toggle.IsOn;
            if (Sub_CnvOption.Passthrough == true)
            {
                Mainwin.mask1.Visibility = mask2.Visibility = mask3.Visibility = Visibility.Visible;
            }
            else
            {
                Mainwin.mask1.Visibility = mask2.Visibility = mask3.Visibility = Visibility.Hidden;
            }
            SetMainCnvOption();
        }

        private void Disable_Control()
        {

        }



        private async void CheckUpdate_Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> downloadUriList = new List<string>();
            List<string> body = new List<string>();
            Mainwin.selected_TextB.Visibility = Visibility.Visible;
            Mainwin.selected_TextB.Text = "アップデートの確認中...";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(
            "User-Agent",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko)");
                client.Timeout = TimeSpan.FromSeconds(5);

                var res = await client.GetAsync("https://api.github.com/repos/falxala/Play-Around-with-Images-2/releases");
                var JsonText = await res.Content.ReadAsStringAsync();

                List<Version> verlist = new List<Version>();

                Version _version;
                dynamic jsonDe = JsonConvert.DeserializeObject(JsonText);
                foreach (var typeStr in jsonDe)
                {
                    try
                    {
                        _version = new Version(Regex.Replace((string)typeStr.tag_name, @"[^0-9^\.]", ""));
                        verlist.Add(_version);
                        body.Add((string)typeStr.body);
                        downloadUriList.Add((string)typeStr.assets[0].browser_download_url);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                int latest_index = verlist.IndexOf(verlist.Max());
                var latest = verlist[latest_index];


                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Reflection.AssemblyName asmName = assembly.GetName();
                System.Version version = asmName.Version;

                Version version1 = new Version(version.ToString());
                Console.WriteLine("cur" + version1 + "|" + latest); ;

                if (version1.CompareTo(latest) >= 0)
                {
                    Mainwin.selected_TextB.Text = ("更新はありません");
                    await Task.Delay(2000);
                }
                else
                {
                    string[] upInfo = {"tag","Describe","uri"};
                    upInfo[0] = latest.ToString();
                    upInfo[1] = body[latest_index];
                    upInfo[2] = "https://github.com/falxala/Play-Around-with-Images-2/releases/tag/v" + latest;

                    Update update = new Update(upInfo);
                    update.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    update.ShowDialog();
                }
            }
            catch
            {
                Mainwin.selected_TextB.Text = ("更新の確認に失敗しました");
                await Task.Delay(2000);
            }
            finally
            {
                Mainwin.selected_TextB.Visibility = Visibility.Hidden;
                Mainwin.selected_TextB.Text = "";
            }
        }
    }
}
