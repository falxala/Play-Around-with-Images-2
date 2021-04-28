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
using Path = System.IO.Path;

namespace PlayAroundwithImages2
{
    /// <summary>
    /// SubWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SubWindow : Window
    {
        //よく使う拡張子だけを抜き出す(enum参照)
        int[] extLsit = new[] { 18, 63, 78, 85, 111, 179, 184, 185, 193, 229, 231, 249 };

        public System.Timers.Timer Timer = new System.Timers.Timer();
        string old_infotext = "";

        System.Windows.Media.Brush ichimatsu;
        public SubWindow()
        {
            InitializeComponent();  
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            Timer.Interval = 100; 
            Timer.AutoReset = true;
        }

        public class ItemSet
        {
            // DisplayMemberとValueMemberにはプロパティで指定する
            public string ItemDisp { get; set; }
            public int ItemValue { get; set; }

            // プロパティをコンストラクタでセット
            public ItemSet(int v, string s)
            {
                ItemDisp = s;
                ItemValue = v;
            }
        }
        public class PathItem
        {
            // DisplayMemberとValueMemberにはプロパティで指定する
            public string ItemDisp { get; set; }
            public string ItemValue { get; set; }

            // プロパティをコンストラクタでセット
            public PathItem(string v, string s)
            {
                ItemDisp = s;
                ItemValue = v;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            old_infotext = Info_TextBox.Text;
            ichimatsu = Mainwin.back_border.Background;

            ComboBox_backgroundColor.Items.Add("Transparent");
            ComboBox_backgroundColor.Items.Add("White");
            ComboBox_backgroundColor.Items.Add("Black");
            ComboBox_backgroundColor.SelectedIndex = 0;

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
            SelectJpeg();//Jpgを選択
            ComboBox_ColorMode.Items.Add(new PathItem("Default", "Default"));
            ComboBox_ColorMode.Items.Add(new PathItem("Remove","Remove"));
            ComboBox_ColorMode.Items.Add(new PathItem("sRGB", ImageMagick.ColorProfile.SRGB.Description));
            ComboBox_ColorMode.Items.Add(new PathItem("CMYK", ImageMagick.ColorProfile.CoatedFOGRA39.Description));
            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(@"ICC", "*.icc", SearchOption.AllDirectories);
                foreach (var icc in files)
                {
                    ComboBox_ColorMode.Items.Add(new PathItem(icc,Path.GetFileNameWithoutExtension(icc)));
                }
            }
            catch { }
            ComboBox_ColorMode.DisplayMemberPath = "ItemDisp";
            ComboBox_ColorMode.SelectedIndex = 0;


            //GPUをコンボボックスにセット
            ComboBox_GPU.Items.Add("Disable");
            ComboBox_GPU.SelectedIndex = 0;

            //DPIをコンボボックスにセット
            ComboBox_DPI.Items.Add(72);
            ComboBox_DPI.Items.Add(144);
            ComboBox_DPI.Items.Add(300);
            ComboBox_DPI.Items.Add(350);
            ComboBox_DPI.Items.Add(600);
            ComboBox_DPI.Items.Add(1200);
            ComboBox_DPI.SelectedIndex = 3;

            ComboBox_PresetDPI.Items.Add(72);
            ComboBox_PresetDPI.Items.Add(144);
            ComboBox_PresetDPI.Items.Add(300);
            ComboBox_PresetDPI.Items.Add(350);
            ComboBox_PresetDPI.Items.Add(600);
            ComboBox_PresetDPI.Items.Add(1200);
            ComboBox_PresetDPI.SelectedIndex = 3;

            for (int i = 0; i <= 10; i++)
                ComboBox_PresetSize.Items.Add(i);

            ComboBox_PresetSize.SelectedIndex = 4;

            ComboBox_PresetSizeAB.Items.Add("A");
            ComboBox_PresetSizeAB.Items.Add("B");
            ComboBox_PresetSizeAB.SelectedIndex = 0;


            for (int i = 10; i > 0; i--)
            {
                ComboBox_opacity.Items.Add(i * 10);
            }


            for (int i = 1; i <= Mainwin.CpuCount; i++)
            {
                ComboBox_CPU.Items.Add(i);
                ComboBox_CPU.SelectedIndex = Mainwin.DegreeOfParallelism - 1;
            }

            if (Mainwin.subDockFlag == false)
                Dock_toggle.IsOn = true;

            selectDirTextBox.Text = Sub_CnvOption.SaveDirectory;

            check_Resources();
            //Info_TextBox.Text = Sub_CnvOption.Transform.ToString();

            Sub_CnvOption.Quality = 75;
            Rotate_TextBox.Text = "0";
            Sub_CnvOption.Rotate = 0;
            Sub_CnvOption.DPI = 350;
            Sub_CnvOption.Rotate = 0;
            Sub_CnvOption.Crop = new int[4] { 0, 0, 0, 0 };
            x.Text = "0";
            y.Text = "0";
            width.Text = "0";
            height.Text = "0";
        }

        public void SelectJpeg()
        {
            foreach (ItemSet Value in ComboBox_extension.Items)
            {
                if (Value.ItemDisp == "Jpg")
                    ComboBox_extension.SelectedIndex = Array.IndexOf(extLsit, Value.ItemValue);
            }
        }
        private void check_Resources()
        {
            ResourcesThreadTxt.Text = ImageMagick.ResourceLimits.Thread.ToString() + " (AUTO)";
            ResourcesDiskTxt.Text = ((double)ImageMagick.ResourceLimits.Disk / 1024 / 1024 / 1024).ToString("0.0") + ("GB (AUTO)");
            ResourcesMemTxt.Text = ((double)ImageMagick.ResourceLimits.Memory / 1024 / 1024 / 1024).ToString("0.0") + ("GB (AUTO)");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        private Process_Image.ConvertOptions sendData;
        internal Process_Image.ConvertOptions Sub_CnvOption = new Process_Image.ConvertOptions();
        public MainWindow Mainwin;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Select_folder.IsEnabled = false;
            // ダイアログのインスタンスを生成
            var dialog = new CommonOpenFileDialog("フォルダーの選択");

            // 選択形式をフォルダースタイルにする IsFolderPicker プロパティを設定
            dialog.IsFolderPicker = true;

            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.InitialDirectory = Sub_CnvOption.SaveDirectory;

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
            ImageMagick.MagickFormat magickFormat = new ImageMagick.MagickFormat();
            try
            {
                magickFormat = (ImageMagick.MagickFormat)((ItemSet)ComboBox_extension.SelectedItem).ItemValue;

                switch (magickFormat)
                {
                    case  ImageMagick.MagickFormat.Jpg:
                        Quality_Border.IsEnabled = true;
                        break;

                    case ImageMagick.MagickFormat.Png:
                        Quality_Border.IsEnabled = true;
                        break;
                    case ImageMagick.MagickFormat.Png64:
                        Quality_Border.IsEnabled = true;
                        break;
                    case ImageMagick.MagickFormat.Png8:
                        Quality_Border.IsEnabled = true;
                        break;

                    default:
                        Quality_Border.IsEnabled = false;
                        break;
                }
            }
            catch
            {
                magickFormat = ImageMagick.MagickFormat.Jpg;
            }
            finally
            {
                Sub_CnvOption.Format = magickFormat;
                if (magickFormat != ImageMagick.MagickFormat.Jpg)
                    Mainwin.mask1_.Visibility = Visibility.Visible;
                else if (!Sub_CnvOption.Passthrough && !Sub_CnvOption.Passthrough)
                    Mainwin.mask1_.Visibility = Visibility.Hidden;

                SetMainCnvOption();
            }
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
            if (Sub_CnvOption.Transform == true && Sub_CnvOption.LiquidRescale == false)
                Estimatedmemory = (product_all * 2 * 4 / 1024 / 1024 + ((long)convetedSize.Width * (long)convetedSize.Height * 2 * 4 / 1024 / 1024) * 2 * (long)count);
            else if (Sub_CnvOption.LiquidRescale)
                //LiquidRescale有効時のメモリ使用量は不明　推測
                Estimatedmemory = (product_all * 2 * 4 / 1024 / 1024 + ((long)convetedSize.Width * (long)convetedSize.Height * 2 * 4 / 1024 / 1024) * 4 * (long)count);
            else
                Estimatedmemory = product_all * 2 * 4 / 1024 / 1024 * 2 + ((long)convetedSize.Width * (long)convetedSize.Height * 2 * 4 / 1024 / 1024) * 2 * (long)count;
            //Q16の場合キャッシュは2バイトなので*2
            Info_TextBox.Text = "最大使用メモリ : " + Estimatedmemory + "MB";
            Info_TextBox.Text += "\r\n FreeMemory : " + FreePhysicalMemory;
            Info_TextBox.Text += " / " + TotalVisibleMemorySize / 1024 + " [MB]";

            deficiencyMemory = Estimatedmemory - FreePhysicalMemory;

            //物理メモリが推定メモリを超過した場合falseを返す
            if (FreePhysicalMemory < Estimatedmemory)
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

        private void selectDirTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sub_CnvOption.SaveDirectory = selectDirTextBox.Text;
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
            Mainwin.Center();
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
            toggleIsOn();
        }

        private void Disable_Control()
        {

        }



        private async void CheckUpdate_Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> downloadUriList = new List<string>();
            List<string> body = new List<string>();
            Mainwin.text_grid.Visibility = Mainwin.selected_TextB.Visibility = Visibility.Visible;
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

                //ソート前のインデックスリスト
                var sorted = verlist
                .Select((x, i) => new KeyValuePair<Version, int>(x, i))
                .OrderBy(x => x.Key)
                .ToList();
                List<int> idx = sorted.Select(x => x.Value).ToList();

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Reflection.AssemblyName asmName = assembly.GetName();
                System.Version version = asmName.Version;

                Version version1 = new Version(version.ToString());
                Info_TextBox.Text = "cur" + version1 + " | last" + latest;
                Console.WriteLine("cur" + version1 + "|last" + latest);

                string merge = "";

                for (int i = idx.Count - 1; i >= 0; i--)
                {
                    merge += body[idx[i]];
                }

                string[] upInfo = { "tag", "Describe", "uri" };
                upInfo[0] = latest.ToString();
                upInfo[1] = merge;
                upInfo[2] = "https://github.com/falxala/Play-Around-with-Images-2/releases/tag/v" + latest;

                if (version1.CompareTo(latest) >= 0)
                {
                    Mainwin.selected_TextB.Text = ("更新はありません");
                    Update update = new Update(upInfo, false);
                    update.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    update.ShowDialog();
                    await Task.Delay(2000);
                }
                else
                {
                    Update update = new Update(upInfo, true);
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
                if (Mainwin.Image_ListView.SelectedItems.Count <= 0)
                    Mainwin.text_grid.Visibility = Visibility.Hidden;
                Mainwin.Image_ListView_SelectionChanged(null, null);
            }
        }


        private void ComboBox_GPU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GPUload)
                foreach (var item in ImageMagick.OpenCL.Devices)
                {
                    if (item.Name == ComboBox_GPU.SelectedItem.ToString())
                        item.IsEnabled = true;
                    else
                        item.IsEnabled = false;
                }
        }

        private void ComboBox_CPU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mainwin.DegreeOfParallelism = ComboBox_CPU.SelectedIndex + 1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            check_Resources();
        }

        private void GrayScale_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.GrayScale = !GrayScale_toggle.IsOn;
            SetMainCnvOption();
            Mainwin.Image_ListView_SelectionChanged(null, null);
        }

        int waitTime = 5;
        private void gamma_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Timer.Start();
            try
            {
                if (timerCount < waitTime)
                    timerCount = 0;

                Sub_CnvOption.Gamma = double.Parse(gamma_TextBox.Text);
                SetMainCnvOption();
            }
            catch
            {
            }
        }

        private void gamma_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9.]").IsMatch(e.Text);
            try
            {
                if (e.Text == "+")
                {
                    var Temp = Sub_CnvOption.Gamma;
                    if (Temp < 10)
                        Temp += 0.1;
                    Sub_CnvOption.Gamma = Temp;

                    gamma_TextBox.Text = Sub_CnvOption.Gamma.ToString();
                    return;
                }
                if (e.Text == "-")
                {
                    var Temp = Sub_CnvOption.Gamma;
                    if (Temp > 0)
                        Temp -= 0.1;
                    Sub_CnvOption.Gamma = Temp;

                    gamma_TextBox.Text = Sub_CnvOption.Gamma.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void gamma_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                gamma_TextBox.Text = Sub_CnvOption.Gamma.ToString();
            }
        }

        private void gamma_TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.Gamma = 1.0;
            gamma_TextBox.Text = Sub_CnvOption.Gamma.ToString("0.0");
            this.IsEnabled = false;
            this.IsEnabled = true;
        }

        private void gamma_TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Sub_CnvOption.Gamma += 0.1;
            if (e.Delta < 0)
                Sub_CnvOption.Gamma -= 0.1;

            if (Sub_CnvOption.Gamma > 10 && e.Delta > 0)
                Sub_CnvOption.Gamma = 10;
            if (Sub_CnvOption.Gamma < 0 && e.Delta < 0)
                Sub_CnvOption.Gamma = 0;

            gamma_TextBox.Text = Sub_CnvOption.Gamma.ToString("0.0");

        }

        private void gamma_gray_changed()
        {
            Mainwin.Image_ListView_SelectionChanged(null, null);
        }
        int timerCount = 0;

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Timer.Stop();
            Timer.Elapsed -= Timer_Elapsed;
            timerCount++;
            //Console.WriteLine(timerCount);
            if (timerCount > waitTime)
            {
                timerCount = 0;
                Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
                UI_change();
                return;
            }
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            Timer.Start();
        }

        private void UI_change()
        {
            if (this.Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(UI_change));
                return;
            }
            this.Dispatcher.Invoke(() =>
            {
                gamma_gray_changed();
            });
        }

        private void openLocalTemp_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ImageMagick"); 
        }

        public void toggleIsOn()
        {
            if (Sub_CnvOption.Passthrough == true)
            {
                Mainwin.mask1.Visibility = Mainwin.mask1_.Visibility = mask2.Visibility = Visibility.Visible;
                Mainwin.Convert_Button.Content = "COPY";
            }
            else
            {
                Mainwin.mask1.Visibility = Mainwin.mask1_.Visibility = mask2.Visibility = Visibility.Hidden;
                ComboBox_extension_SelectionChanged(null,null);
                Mainwin.Convert_Button.Content = "CONVERT";
            }
            SetMainCnvOption();
        }

        private void NotScaleUp_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.NotScaleUp = !NotScaleUp_toggle.IsOn;
            SetMainCnvOption();
        }

        bool GPUload = false;
        private async void ComboBox_GPU_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!File.Exists(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ImageMagick\\ImagemagickOpenCLDeviceProfile.xml"))
            {
                Mainwin.selected_TextB.Visibility = Visibility.Visible;
                Mainwin.text_grid.Visibility = Visibility.Visible;
                Mainwin.selected_TextB.Text = "OpenCL\r\n初回ベンチマーク実行中";
                await Task.Delay(10);
            }
            if (!GPUload)
            {
                foreach (var item in ImageMagick.OpenCL.Devices)
                {
                    ComboBox_GPU.Items.Add(item.Name);
                }
                GPUload = true;
                await Task.Delay(10);
                Mainwin.selected_TextB.Visibility = Visibility.Hidden;
                await Task.Delay(10);
                Mainwin.Image_ListView_SelectionChanged(null,null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Mainwin.back_border.Background = new SolidColorBrush(Colors.White);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_backgroundColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_backgroundColor.SelectedIndex)
            {
                case 0:
                    Mainwin.back_border.Background = ichimatsu;
                    Sub_CnvOption.BackgroundColor = new ImageMagick.MagickColor("transparent");
                    break;
                case 1:
                    Mainwin.back_border.Background = new SolidColorBrush(Colors.White);
                    Sub_CnvOption.BackgroundColor = new ImageMagick.MagickColor("white");
                    break;
                case 2:
                    Mainwin.back_border.Background = new SolidColorBrush(Colors.Black);
                    Sub_CnvOption.BackgroundColor = new ImageMagick.MagickColor("black");
                    break;
            }
            SetMainCnvOption();
            
        }

        private void ComboBox_DPI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sub_CnvOption.DPI = (int)ComboBox_DPI.SelectedItem;
            SetMainCnvOption();
        }

        private void Dock_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
                Mainwin.subDockFlag = !Dock_toggle.IsOn;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
                this.Hide();
            }
        }

        private void Quality_TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Quality_TextBox.Text = "75";
            this.IsEnabled = false;
            this.IsEnabled = true;
        }

        private void Quality_TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Sub_CnvOption.Quality += 1;
            if (e.Delta < 0)
                Sub_CnvOption.Quality -= 1;

            if (Sub_CnvOption.Quality > 100 && e.Delta > 0)
                Sub_CnvOption.Quality = 100;
            if (Sub_CnvOption.Quality <= 1 && e.Delta < 0)
                Sub_CnvOption.Quality = 1;

            Quality_TextBox.Text = Sub_CnvOption.Quality.ToString();
            SetMainCnvOption();
        }

        private void Quality_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Quality_TextBox.Text = Sub_CnvOption.Quality.ToString();
            }
        }

        private void Quality_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
            try
            {
                if (e.Text == "+")
                {
                    var Temp = Sub_CnvOption.Quality;
                    if (Temp < 100)
                        Temp ++;
                    Sub_CnvOption.Quality = Temp;

                    Quality_TextBox.Text = Sub_CnvOption.Quality.ToString();
                    return;
                }
                if (e.Text == "-")
                {
                    var Temp = Sub_CnvOption.Quality;
                    if (Temp > 1)
                        Temp --;
                    Sub_CnvOption.Quality = Temp;

                    Quality_TextBox.Text = Sub_CnvOption.Quality.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Quality_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (int.Parse(Quality_TextBox.Text) >= 1 && int.Parse(Quality_TextBox.Text) <= 100)
                    Sub_CnvOption.Quality = int.Parse(Quality_TextBox.Text);
                SetMainCnvOption();
            }
            catch { }
        }

        private void crop_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Sub_CnvOption.Crop[0] = int.Parse(x.Text);
                Sub_CnvOption.Crop[1] = int.Parse(y.Text);
                Sub_CnvOption.Crop[2] = int.Parse(width.Text);
                Sub_CnvOption.Crop[3] = int.Parse(height.Text);
                SetMainCnvOption();
            }
            catch {
            }
        }

        private void Overwrite_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.Overwrite = !Overwrite_toggle.IsOn;
            SetMainCnvOption();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ComboBox_opacity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Opacity = double.Parse(ComboBox_opacity.SelectedItem.ToString()) / 100;
        }

        public void Tranceform_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.Transform = !Tranceform_toggle.IsOn;
            if(Sub_CnvOption.Transform == true)
            {
                Sub_CnvOption.Transform = true;
                Width_TextBox.IsEnabled = true;
                Height_TextBox.IsEnabled = true;
                Mainwin.limit_longside_tb.IsEnabled = false;
            }
            else
            {
                Sub_CnvOption.Transform = false;
                Width_TextBox.IsEnabled = false;
                Height_TextBox.IsEnabled = false;
                Mainwin.limit_longside_tb.IsEnabled = true;
            }
            SetMainCnvOption();
        }

        private void support_blender_toggle_Copy_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (support_blender_toggle.IsOn)
            {
                Sub_CnvOption.BlenderPath = "";
            }
            else
            {
                var blenderpath = SerchBlender.GetUninstallList();
                if (blenderpath != null)
                    Sub_CnvOption.BlenderPath = blenderpath[0];
                else
                    Sub_CnvOption.BlenderPath = null;
                SetMainCnvOption();
            }
        }

        private void ColorSeparation_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Sub_CnvOption.ColorSeparation = !ColorSeparation_toggle.IsOn;
            SetMainCnvOption();
        }

        System.Windows.Size A0 = new System.Windows.Size(841, 1189);
        System.Windows.Size B0 = new System.Windows.Size(1030, 1456);
        System.Windows.Size C0 = new System.Windows.Size(917, 1297);
        private const double inch = 25.4;

        private void Preset_Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            PresetAply();
            Mainwin.limit_longside_tb.Text = Math.Max(p.Width, p.Height).ToString();
            Width_TextBox.Text = p.Width.ToString();
            Height_TextBox.Text = p.Height.ToString();
            var TempSize = Sub_CnvOption.Size;
            TempSize.Width = (int)p.Width;
            TempSize.Height = (int)p.Height;
            Sub_CnvOption.Size = TempSize;
            SetMainCnvOption();

        }

        Preset p = new Preset();
        private void PresetAply()
        {
            var dpi = int.Parse(ComboBox_PresetDPI.SelectedItem.ToString());
            var comboIndex = ComboBox_PresetSize.SelectedIndex;
            if (ComboBox_PresetSizeAB.SelectedIndex == 0)    //A列
            {
                p = PresetSize(A0.Width, A0.Height, comboIndex, dpi);
            }
            if (ComboBox_PresetSizeAB.SelectedIndex == 1)    //B列
            {
                p = PresetSize(B0.Width, B0.Height, comboIndex, dpi);
            }
            PresetTB.Text = $"{p.Width} px × {p.Height} px \r\n{p.ActualWidth} mm × {p.ActualHeight} mm";
        }

        public class Preset
        {
            public double Width { get; set; }
            public double Height { get; set; }
            public double ActualWidth { get; set; }
            public double ActualHeight { get; set; }
        }

        private Preset PresetSize(double width, double height, int series_num, int dpi)
        {
            Preset preset = new Preset();
            if ((ComboBox_PresetSize.SelectedIndex) % 2 == 0)　     //偶数列
            {
                preset.ActualWidth = RoundDivision(width, series_num / 2);
                preset.ActualHeight = RoundDivision(height, series_num / 2);
                preset.Width = Math.Round(dpi * preset.ActualWidth / inch);
                preset.Height = Math.Round(dpi * preset.ActualHeight / inch);
            }
            else                                                    //奇数列
            {
                //縦横を保持させるために反転
                preset.ActualHeight = RoundDivision(width, series_num / 2);
                preset.ActualWidth = RoundDivision(Math.Round(height / 2), series_num / 2);
                preset.Width = Math.Round(dpi * preset.ActualWidth / inch);
                preset.Height = Math.Round(dpi * preset.ActualHeight / inch);
            }
            return preset;
        }

        /// <summary>
        /// 累積誤差を含む除算
        /// </summary>
        /// <param name="value"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private double RoundDivision(double value, int N)
        {
            for (int i = 0; i < N; i++)
                value = Math.Floor(value / 2);
            return value;
        }

        private void ComboBox_PresetSizeAB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PresetAply();
        }

        private void ComboBox_PresetSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PresetAply();
        }

        private void ComboBox_PresetDPI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PresetAply();
        }

        private void CheckReadme_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/falxala/Play-Around-with-Images-2#play-around-with-images-2");
        }

        private void OpenFolder_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", Sub_CnvOption.SaveDirectory);
        }

        private void ComboBox_ColorMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sub_CnvOption.ColorMode = ((PathItem)ComboBox_ColorMode.SelectedItem).ItemValue;
            SetMainCnvOption();
        }

        private void ClipBoard_toggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ClipBoard_toggle.IsOn == true)
                Mainwin.monitor.Stop();
            else
                Mainwin.monitor.Start();
        }
    }
}