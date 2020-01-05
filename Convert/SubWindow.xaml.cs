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
// Windows API Code Pack のダイアログの名前空間を using
using Microsoft.WindowsAPICodePack.Dialogs;

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
            var extLsit = new[] { 6, 15, 50, 71, 87, 102, 168, 181, 217, 235 };

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
            tf_checkbox.IsChecked = false;

            selectDirTextBox.Text = Sub_CnvOption.SaveDirectory;
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

        private void Window_LocationChanged(object sender, EventArgs e)
        {

            
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

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
            var img = new Image();
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
            SetMainCnvOption();
        }

        private void tf_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Sub_CnvOption.Transform = false;
            Width_TextBox.IsEnabled = false;
            Height_TextBox.IsEnabled = false;
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
    }
}
