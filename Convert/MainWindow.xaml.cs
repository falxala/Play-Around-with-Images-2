﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using Dasync.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using Reactive.Bindings;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace PlayAroundwithImages2
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<Model.drop_Image> drop_Images = new ObservableCollection<Model.drop_Image>(); // コレクションのインスタンスを作る。

        ViewModel preview_model = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(this.drop_Images, new object());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ImageMagick.OpenCL.IsEnabled = true;

            Image_ListView.ItemsSource = drop_Images; // コレクションをListBoxにバインドする
            //preview_image.DataContext = Subwin.Rotate_Slider;

            preview_model.Rotate.Value = 0;
            preview_model.Scale_X.Value = 1;
            preview_model.Scale_Y.Value = 1;
            preview_image.DataContext = preview_model;

            //selected_TextB.Visibility = text_grid.Visibility = Visibility.Hidden;
            var window = Window.GetWindow(this);
            window.KeyDown += HandleKeyPress;
            selected_TextB.Text = "";
            MaxDegreeOfParallelism(70.0);
            cnvOption.Format = ImageMagick.MagickFormat.Jpeg;

            cnvOption.SaveDirectory = System.Environment.CurrentDirectory + "\\outputs";
            cnvOption.Transform = false;
        }

        //一時保存パス
        string TempPath = System.Environment.CurrentDirectory + "\\_TEMP_paiclip";

        readonly int CpuCount = Environment.ProcessorCount;
        //同時変換数
        public int DegreeOfParallelism = 1;

        /// <summary>
        ///　同時使用CPUスレッド数を設定
        /// </summary>
        /// <param name="Percentage"></param>
        private void MaxDegreeOfParallelism<Type>(Type Percentage)
          where Type : IComparable
        {
            if (typeof(Type) == typeof(Double))
            {
                double _Percentage = (double)Convert.ChangeType(Percentage, typeof(Double));
                if (0 <= _Percentage && _Percentage <= 100)
                {
                    DegreeOfParallelism = (int)Math.Ceiling(CpuCount * _Percentage / 100);
                }
                else
                {
                    DegreeOfParallelism = 1;
                }
            }
        }

        public struct PasteData
        {
            public IDataObject data { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }

        private void Image_ListView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                PasteData pasteData = new PasteData();
                if (e.Data.GetDataPresent("UniformResourceLocator"))
                    throw new Exception("URL非対応");
                pasteData.data = e.Data;
                ListviewDropPaste(pasteData);

            }
            catch { }
        }

        private async void ListviewFileLoad(string[] ItemPath)
        {
            Working_Flag = true;
            Clear_info();
            drop_grid.Visibility = Visibility.Hidden;

            List<string> files = new List<string>();


            try
            {
                //ドロップアイテム個々のパスを取得
                foreach (string dropItem in ItemPath)
                {

                    //ドロップがディレクトリの場合
                    if (System.IO.Directory.Exists(dropItem))
                    {
                        //フォルダ内のファイル名を取得|サブフォルダ内も全て検索
                        files.AddRange(System.IO.Directory.GetFiles(@dropItem, "*", System.IO.SearchOption.AllDirectories));


                    }
                    //ドロップがディレクトリ以外
                    else
                    {
                        files.Add(System.IO.Path.GetFullPath(dropItem));
                    }

                }


                //縮小表示
                if (files.Count >= 10 && Image_ListView.Items.Count == 0)
                {
                    Tslider.Value = 125;
                }
                if (files.Count >= 25 && Image_ListView.Items.Count == 0)
                {
                    Tslider.Value = 100;
                }
                if (files.Count >= 100 && Image_ListView.Items.Count == 0)
                {
                    Tslider.Value = 50;
                }

                //ファイル数制限
                if (files.Count >= 2048)
                {
                    selected_TextB.Text = "file count limit";
                    throw new Exception("file count limit");
                }
                Convert_Button.IsEnabled = false;
                int errorCount = 0;
                await Task.Run(() =>
                {
                    errorCount = Data_reading(files.ToArray());
                });
                Convert_Button.IsEnabled = true;
                selected_TextB.Text = "読み込み完了";
                if (errorCount != 0)
                {
                    selected_TextB.Text += "\r\n<" + errorCount + "個の非対応ファイル" + ">";
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.InnerException.ToString());

                selected_TextB.Visibility = text_grid.Visibility = Visibility.Visible;
                if (typeof(ImageMagick.MagickMissingDelegateErrorException) == ex.GetType())
                {
                }
            }
            finally
            {
                Working_Flag = false;
                await Task.Delay(100);
                if (Image_ListView.Items.Count == 0)
                {
                    drop_grid.Visibility = Visibility.Visible;
                }
            }
        }


            /// <summary>
            /// 処理中
            /// </summary>
            bool Working_Flag = false;
        private  void ListviewDropPaste(PasteData pasteData)
        {


            if (pasteData.path != null)
                ListviewFileLoad(new string[] { pasteData.path });
            else
            {

                //ドロップアイテム全てのパスを取得
                string[] dropItemPath = (string[])pasteData.data.GetData(DataFormats.FileDrop, false);
                ListviewFileLoad(dropItemPath);

            }
        }

        bool disable_thumbnail = false;

        public IReactiveProperty<BitmapImage> noimage { get; } = new ReactiveProperty<BitmapImage>(new BitmapImage(new Uri("pack://application:,,,/PlayAroundwithImages2;component/Resources/noimage.png")));

        /// <summary>
        /// 配列のファイルを読み込む
        /// </summary>
        /// <param name="files">ファイルパス</param>
        int Data_reading(string[] files)
        {

            string[] details = new string[9];
            BitmapImage bitmap = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                bitmap = noimage.Value;
            });

            int Count = 0;
            int Error = 0;
            Process_Image process_Image = new Process_Image();
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = DegreeOfParallelism;
            Console.WriteLine(DegreeOfParallelism);
            Parallel.For(0, files.Length, options, i =>
             {
                 try
                 {
                     ///tryで応急処置
                     ///あとで直すこと！！！
                     //例外が発生すると最後まで画像が読み込まれない
                     try
                     {
                         string file = files[i];


                         //許可されている拡張子以外は読み込まない
                         bool check = false;
                         foreach (string name in Enum.GetNames(typeof(permission.Extension)))
                         {
                             if (String.Compare(System.IO.Path.GetExtension(file), "." + name, true) == 0)
                             {
                                 check = true;
                             }
                         }
                         if (check == false)
                             throw new Exception();

                         details = process_Image.GetDetail(file);

                         Model.drop_Image drop_Image = new Model.drop_Image();
                         drop_Image.Image_path = file;
                         drop_Image.File_name = System.IO.Path.GetFileName(file);
                         if (disable_thumbnail == false)
                         {
                             drop_Image.thumbnail = Process_Image.Create_thumbnail(file);
                             drop_Image.Create_thumunail = true;
                         }
                         else
                         {
                             drop_Image.thumbnail = bitmap;
                             drop_Image.Create_thumunail = false;
                         }
                         FileInfo file_size = new FileInfo(file);
                         drop_Image.ColorSpace = details[1];
                         drop_Image.Icc = details[8];
                         drop_Image.File_size = file_size.Length;
                         drop_Image.Image_size = new System.Drawing.Size(int.Parse(details[2]), int.Parse(details[3]));
                         drop_Image.Format = details[4];
                         drop_Images.Add(drop_Image);
                         Count++;
                     }
                     catch { Error++; }
                 }
                 catch
                 {
                     //Console.WriteLine(ex.Message);

                     if (Image_ListView.Items.Count <= 0)
                     {
                         drop_grid.Visibility = Visibility.Visible;
                     }
                 }
                 finally
                 {
                     //読み込み進捗表示
                     this.Dispatcher.Invoke((Action)(() =>
                     {
                         selected_TextB.Visibility = text_grid.Visibility = Visibility.Visible;
                         Progress(Count, files.Length, "NOW LOADING\r\n");
                     }));

                 }

             });

            if (Error > 0)
            {
                return Error;
            }
            return 0;

        }

        private void Image_ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }


        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Deselect_All_Button_Click(null, null);
        }

        void it_MouseEnter(object sender, MouseEventArgs e)
        {
            ///Sender is the required ListBoxItem
        }

        private void buttonDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (drop_Images.Count < 1) return; // コレクションの数が0の場合は何もしない。

                if (Image_ListView.SelectedItems.Count == Image_ListView.Items.Count)
                {
                    drop_Images.Clear();
                    Image_ListView.ClearValue(ListView.ItemsSourceProperty);
                    Image_ListView.ItemsSource = drop_Images;
                    drop_grid.Visibility = Visibility.Visible;
                    return;
                }

                // ListBoxで選択されたアイテムを、別のコレクションにコピーする。
                Collection<Model.drop_Image> selected = new Collection<Model.drop_Image>();
                foreach (Model.drop_Image i in Image_ListView.SelectedItems)
                {
                    selected.Add(i);
                }

                Clear_info();

                // 元のコレクションから、選択されたアイテムと同じアイテムを削除する。

                foreach (Model.drop_Image item in selected)
                {
                    drop_Images.Remove(item);
                }

                if (Image_ListView.Items.Count <= 0)
                {
                    drop_grid.Visibility = Visibility.Visible;
                }
            }
            catch { }
            finally
            {
                GC();
            }

        }

        void GC()
        {
            System.GC.Collect(); // アクセス不可能なオブジェクトを除去
            System.GC.WaitForPendingFinalizers(); // ファイナライゼーションが終わるまでスレッド待機
            System.GC.Collect(); // ファイナライズされたばかりのオブジェクトに関連するメモリを開放
        }

        void Clear_info()
        {
            preview_image.Source = null;
            path_textB.Text = "[Path]";
            Detail_textB.Text = "[Details]";
            selected_TextB.Visibility = Visibility.Hidden;
            text_grid.Visibility = Visibility.Visible;
        }

        private void Select_All_Button_Click(object sender, RoutedEventArgs e)
        {
            Image_ListView.Focus();
            Image_ListView.SelectAll();
        }

        private void Deselect_All_Button_Click(object sender, RoutedEventArgs e)
        {
            Image_ListView.Focus();
            Image_ListView.SelectedIndex = -1;
            Clear_info();
        }
        internal void Image_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //処理中なら何もしない
            if (Working_Flag == true)
                return;
            try
            {

                if (Image_ListView.SelectedItems.Count > 1)
                {
                    Clear_info();
                    selected_TextB.Visibility = text_grid.Visibility = Visibility.Visible;
                    selected_TextB.Text = Image_ListView.SelectedItems.Count + " selected";
                    return;
                }
                else if (Image_ListView.SelectedItems.Count == 0)
                {
                    Clear_info();
                    return;
                }
                int w, h, gcd;
                foreach (Model.drop_Image selected_item in Image_ListView.SelectedItems)
                {
                    w = selected_item.Image_size.Width;
                    h = selected_item.Image_size.Height;
                    gcd = Gcd(w, h);
                    selected_TextB.Visibility = text_grid.Visibility = Visibility.Hidden;

                    if (selected_item.Create_thumunail == false)
                    {
                        selected_item.thumbnail = Process_Image.Create_thumbnail(selected_item.Image_path);
                        selected_item.Create_thumunail = true;
                        Image_ListView.Items.Refresh();
                    }
                    preview_image.Source = selected_item.thumbnail;
                    
                    path_textB.Text = "[Path]\r\n" + selected_item.Image_path;
                    Detail_textB.Text = "[Details]\r\n" + w + "*" + h;
                    Detail_textB.Text += " | " + w / gcd + " : " + h / gcd;
                    Detail_textB.Text += "  |  " + ((float)selected_item.File_size / 1024 / 1024).ToString("F2") + " [MB]\r\n";
                    Detail_textB.Text += "Format : " + selected_item.Format + " | " + "ColorSpace : " + selected_item.ColorSpace + "\r\n";
                    Detail_textB.Text += "ICC : " + selected_item.Icc;
                }
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }

        //最大公約を求めるメソッド
        public static int Gcd(int a, int b)
        {
            Func<int, int, int> gcd = null;
            gcd = (x, y) => y == 0 ? x : gcd(y, x % y);
            return a > b ? gcd(a, b) : gcd(b, a);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double num = Slider1.Value;
            if (num <= 10)
                limit_filesize_tb.Text = num.ToString("F2");
        }

        public void Progress(int n,int Count,String addText)
        {
            selected_TextB.Text = addText;
            selected_TextB.Text += ((float)n / Count * 100).ToString("F2") + "%" + " ";
            selected_TextB.Text += "[" + n + "/" + Count + "]";
        }

        public Process_Image.ConvertOptions cnvOption = new Process_Image.ConvertOptions();

        private CancellationTokenSource tokenSource = null;
        private async void Convert_Button_Click(object sender, RoutedEventArgs e)
        {

            //処理中に実行させない
            if (Working_Flag)
            {
                Stop();
                Convert_Button.Content = "END PROCESSING...";
                return;
            }

            Subwin.CheckMemory(out long deficiencyMemory, out bool checkValue, Image_ListView.SelectedItems, DegreeOfParallelism, cnvOption.Size);
            //メモリチェック
            if (!checkValue)
            {
                MessageBoxResult result = MessageBox.Show("物理メモリが " + ((float)deficiencyMemory / 1024).ToString("F2") + " GB " + "不足しています\r\nストレージを使用して続行しますか？",
                "警告", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel);

                if (result == MessageBoxResult.Cancel)
                    return;
            }

            Working_Flag = true;
            Convert_Button.Content = "STOP";

            Clear_info();

            if (Subwin.Visibility == Visibility.Hidden)
                Set_Option();

            System.Collections.Specialized.StringCollection outputFileNames
                = new System.Collections.Specialized.StringCollection();

            try
            {
                if (Image_ListView.SelectedItems.Count <= 0)
                {
                    throw new MyException("アイテムが選択されていません");
                }

                Process_Image process_Image = new Process_Image();
                //変換中にコレクションに操作があった場合例外となるためコピーしておく
                Collection<Model.drop_Image> selected = new Collection<Model.drop_Image>();
                foreach (Model.drop_Image i in Image_ListView.SelectedItems)
                {
                    selected.Add(i);
                }
         
                selected_TextB.Visibility = text_grid.Visibility = Visibility.Visible;
                Progress(outputFileNames.Count, selected.Count, "PROCESSING\r\n");

                using (this.tokenSource = new CancellationTokenSource())
                {
                    await selected.ParallelForEachAsync(async item =>
                {
                    var Result = await process_Image.Convert(item, cnvOption, tokenSource.Token);
                    outputFileNames.Add(Result);
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        Progress(outputFileNames.Count, selected.Count, "PROCESSING\r\n");
                        if (selected.Count == outputFileNames.Count)
                            Progress(outputFileNames.Count, selected.Count, "COMPLETE\r\n");
                    }));

                }, maxDegreeOfParallelism: DegreeOfParallelism);
                }
                selected.Clear();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);

                foreach (var innnerExc in ex.InnerExceptions)
                {
                    Console.WriteLine(ex.GetType());
                    if (ex.GetType() == typeof(Dasync.Collections.ParallelForEachException))
                    {
                        selected_TextB.Visibility = text_grid.Visibility = Visibility.Visible;
                        selected_TextB.Text = "変換エラー\r\n";
                        selected_TextB.Text += "詳細 : " + innnerExc.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(MyException))
                {
                    selected_TextB.Visibility = text_grid.Visibility = Visibility.Visible;
                    selected_TextB.Text = "最低一つ以上のファイルを\r\n選択してください";
                }
                Console.WriteLine(ex.Message);
                
            }
            finally
            {
                Working_Flag = false;
                if (cnvOption.Passthrough)
                    Convert_Button.Content = "COPY";
                else
                    Convert_Button.Content = "CONVERT";
            }

            try
            {
                Clipboard.Clear();
                if (outputFileNames.Count > 1)
                    Clipboard.SetFileDropList(outputFileNames);
                else
                {               
                    var filePath = outputFileNames[0];
                    //DataObjectオブジェクトを作成し、FileDrop形式のデータを追加する
                    DataObject data = new DataObject(DataFormats.FileDrop, new string[] { filePath });
                    //Bitmap形式のデータを追加する
                    Bitmap bmp = new Bitmap(filePath);
                    data.SetData(DataFormats.Bitmap, bmp);
                    //クリップボードに貼り付ける
                    Clipboard.SetDataObject(data, true);
                    //後始末
                    bmp.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.GetType() == typeof(ArgumentException) && outputFileNames.Count != 0)
                {
                    selected_TextB.Text = "クリップボードへのコピーに失敗しました";
                }
            }
            finally
            {
                await Task.Delay(2000);
                selected_TextB.Text = "";
                Image_ListView_SelectionChanged(null,null);
            }
        }

        private void Slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int longside = (int)Slider2.Value;
            if (longside < 8192)
                limit_longside_tb.Text = longside.ToString();
            if (longside == 8192)
                limit_longside_tb.Text = longside.ToString();
        }

        private void limit_filesize_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSliderFromText(Slider1, limit_filesize_tb, Slider1.Maximum);
            
            //JPEGを選択
            Subwin.ComboBox_extension.SelectedIndex = 5;
        }

        private void limit_longside_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSliderFromText(Slider2, limit_longside_tb, Slider2.Maximum);
            Subwin.tf_checkbox.IsChecked = false;
            
            //longsideが0ならNONEに
            if(cnvOption.Size.Width == 0)
            {
                limit_longside_tb.Text = "Original";
            }
        }

        /// <summary>
        /// テキストボックスの値をスライダーに設定する
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        private void SetSliderFromText (Slider slider, TextBox textBox, double limit)
        {
            try
            {
                var value = double.Parse(textBox.Text);
                if (limit >= value)
                    slider.Value = value;
                Set_Option();
                if (Subwin.Visibility == Visibility.Visible)
                    Subwin.SendData = cnvOption;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Image_ListView_MouseEnter(object sender, MouseEventArgs e)
        {

            foreach (var item in Image_ListView.Items)
            {
                var container = Image_ListView.ItemContainerGenerator.ContainerFromItem(item);

            }
        }

        private void ListView_CntxtMenu_paste_Click(object sender, RoutedEventArgs e)
        {
            CopyFromClipboard();
        }

        private void OnCtrlV(object sender, ExecutedRoutedEventArgs e)
        {
            CopyFromClipboard();
        }

        private void CopyFromClipboard()
        {
            PasteData pasteData = new PasteData();
            if (Clipboard.ContainsFileDropList())
            {
                pasteData.data = Clipboard.GetDataObject();
                ListviewDropPaste(pasteData);
            }
            //クリップボードの画像を取り込む
            else if (Clipboard.ContainsImage())
            {
                //クリップボードにあるデータの取得
                BitmapSource img = Clipboard.GetImage();
                if (img != null)
                {

                    if (!Directory.Exists(TempPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(TempPath);
                        di.Create();
                    }

                    string filePath = CreateName(TempPath + "\\clip") + ".png";
                    //ファイルに保存
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(img));
                        encoder.Save(fileStream);
                    }

                    pasteData.path = filePath;
                    ListviewDropPaste(pasteData);

                }
            }
        }

        //連番ファイル名を作成
        public static string CreateName(string path)
        {
            int i = 1;
            var newPath = path;
            while (File.Exists(newPath+ ".png"))
            {
                newPath = $"{path} ({i++})";
            }
            return newPath;
        }

        private void OnDel(object sender, ExecutedRoutedEventArgs e)
        {
            buttonDel_Click(null,null);
        }

        private void OnCtrlA(object sender, ExecutedRoutedEventArgs e)
        {
            Select_All_Button_Click(null, null);
        }


        /// <summary>
        /// 選択されたファイルを選択した状態でフォルダを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_CntxtMenu_open_Click(object sender, RoutedEventArgs e)
        {
            List<string> ExistingPAth = new List<string>();

            //複数フォルダ事前チェック
            foreach (Model.drop_Image item in Image_ListView.SelectedItems)
            {
                if (ExistingPAth.Contains(System.IO.Path.GetDirectoryName(item.Image_path)) == false)
                {
                    ExistingPAth.Add(System.IO.Path.GetDirectoryName(item.Image_path));
                }
            }

            if(ExistingPAth.Count > 4)
            {
                MessageBoxResult result = MessageBox.Show(ExistingPAth.Count + "個のエクスプローラーを開きます\r\nよろしいですか？",
                "警告", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.Cancel)
                    return;
            }

            ExistingPAth.Clear();
            foreach (Model.drop_Image item in Image_ListView.SelectedItems)
            {
                if (ExistingPAth.Contains(System.IO.Path.GetDirectoryName(item.Image_path)) == false)
                {
                    System.Diagnostics.Process.Start("EXPLORER.EXE", @"/select," + item.Image_path);
                    ExistingPAth.Add(System.IO.Path.GetDirectoryName(item.Image_path));
                }
            }            
        }


        //Window間データ受け渡し
        private Process_Image.ConvertOptions receiveData;
        public Process_Image.ConvertOptions ReceiveData
        {
            set
            {
                cnvOption = value;
                receiveData = value;

                preview_model.Rotate.Value = cnvOption.Rotate;
                if (cnvOption.Mirror && 0 < preview_model.Scale_X.Value)
                    preview_model.Scale_X.Value = (double)preview_model.Scale_X.Value * -1;
                else if (!cnvOption.Mirror && 0 > preview_model.Scale_X.Value)
                    preview_model.Scale_X.Value = (double)preview_model.Scale_X.Value * -1;


            }
            get
            {
                return receiveData;
            }
        }

        SubWindow Subwin = new SubWindow();
        private void SettingWindow_Button_Click(object sender, RoutedEventArgs e)
        {
            Window_LocationChanged(sender, e);
            //インスタンスを探してWindow表示
            var a = Application.Current.Windows.OfType<SubWindow>().SingleOrDefault(w => w.Owner == this);
            if (a != null)
            {
                if (a.Visibility == Visibility.Visible)
                {
                    a.Visibility = Visibility.Hidden;
                    //SettingWindow_Button.Content = ">>";
                    return;
                }
                else
                {
                    Set_Option();
                }

                //SettingWindow_Button.Content = "<<";
                a.Visibility = Visibility.Visible;
                Subwin.SendData = cnvOption;                
            }
            else
            {
                Subwin.SendData = cnvOption;
                //SettingWindow_Button.Content = "<<";
                Subwin.Owner = this;
                Subwin.Mainwin = this;
                Subwin.Show();
                //Dock有効
                Subwin.Dock_checkbox.IsChecked = true;
            }

        }

        //起動時Dock設定有効
        public　bool subDockFlag = true;
        /// <summary>
        /// メインウィンドウにサブウィンドウを追従させる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (subDockFlag)
            {
                var point = Mouse.GetPosition(this);
                var winpos = new System.Windows.Point(this.Left, this.Top);

                point.X = winpos.X + (int)this.Width;
                point.Y = winpos.Y;

                Subwin.Left = point.X;
                Subwin.Top = point.Y;

                Subwin.Height = this.Height;
            }
        }

        private void Set_Option()
        {
            cnvOption.Filesize = Double.Parse(limit_filesize_tb.Text);
            if (limit_longside_tb.Text != "Original")
                cnvOption.Size = new System.Drawing.Size(Int32.Parse(limit_longside_tb.Text), Int32.Parse(limit_longside_tb.Text));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized && subDockFlag)
            {
                //subDockFlag = false;
                //Subwin.Dock_checkbox.IsChecked = false;
                Subwin.Top = this.Top;
            }
            Window_LocationChanged(sender, e);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //スクリーンショットの一時フォルダを削除する
            System.IO.DirectoryInfo tempdi = new System.IO.DirectoryInfo(TempPath);
            if (tempdi.Exists)
                tempdi.Delete(true);

            //削除されなかったmagicktempファイルを削除する
            string[] magick_TEMP = Directory.GetFiles(Path.GetTempPath(), @"magick*");
            foreach(var item in magick_TEMP)
            {
                System.IO.File.Delete(item);
            }

            //表示されていないインスタンスを表示し閉じる
            var notShown = Application.Current.Windows.OfType<SubWindow>().SingleOrDefault(w => true);
            if (notShown != null)
            {
                Subwin.Owner = this;
                Subwin.Mainwin = this;
                notShown.Hide();
            }

        }

        private void limit_longside_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //数字以外が入力されたらキャンセル
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void limit_filesize_tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]||[.]").IsMatch(e.Text);
        }

        //変換中止
        private void Stop()
        {
            if (tokenSource == null) { return; }

            selected_TextB.Text = "Stop of the processing \r\nPlease wait a moment";
            tokenSource.Cancel();
        }

        private void ToggleSwitch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Multiple_toggle.IsOn)
            {
                Image_ListView.SelectionMode = SelectionMode.Extended;
            }
            else
            {
                Image_ListView.SelectionMode = SelectionMode.Multiple;
            }
        }

        private void Image_ListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(Image_ListView.SelectedItems.Count == 1)
            {

                foreach (Model.drop_Image i in Image_ListView.SelectedItems)
                {
                    System.Diagnostics.Process p = System.Diagnostics.Process.Start(i.Image_path);
                }
            }
        }

        private async void Tslider_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await Task.Delay(5);
            Tslider.Value = 200;
        }

        private void Slider2_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                Slider2.Value++;
            }
            else if(e.Delta < 0)
            {
                Slider2.Value--;
            }
            
        }

        private void Slider1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Slider1.Value++;
            }
            else if (e.Delta < 0)
            {
                Slider1.Value--;
            }
        }

        private void Image_ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
            {
                if(e.Delta > 0)
                {
                    //ルートより少し小さい値を入れる
                    //丸め込み有効
                    Tslider.Value = Math.Round(((Tslider.Value * 1.38) / 100), 1) * 100;
                }
                else if(e.Delta < 0)
                {
                    Tslider.Value = Math.Round(((Tslider.Value / 1.38) / 100), 1) * 100;
                }
                
            }
        }

        private void context_thumbnail_Checked(object sender, RoutedEventArgs e)
        {
            disable_thumbnail = true;
        }

        private void context_thumbnail_Unchecked(object sender, RoutedEventArgs e)
        {
            disable_thumbnail = false;
        }

        //テキストコントロールダブルクリック
        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddFile(false);
        }

        private void AddFile(bool isFolder)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "開く",
                // フォルダ選択
                IsFolderPicker = isFolder,
                //InitialDirectory = "",
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                //DefaultDirectory = "",
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = true,
                EnsureValidNames = true,
                Multiselect = true,
                ShowPlacesList = true
            };

            if (dialog.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                ListviewFileLoad(dialog.FileNames.ToArray());
            }

            this.IsEnabled = true;
        }

        private void _AddFile(object sender, RoutedEventArgs e)
        {
            AddFile(false);
        }

    }
}
