using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayAroundwithImages2
{
    public class Model
    {
        public class drop_Image
        {
            public ListBoxItem ListBoxItem { get; set; }
            public string Image_path { get; set; }
            public string File_name { get; set; }
            public long File_size { get; set; }
            public bool Check { get; set; }
            public System.Drawing.Size Image_size { get; set; }
            public BitmapSource thumbnail { get; set; }
            public string ColorSpace { get; set; }
            public string Icc { get; set; }
            public string Format { get; set; }
            public bool Create_thumunail { get; set; }
            public float Gamma { get; set; }
            public short Bit { get; set; }
            public int Number { get; set; }
        }

    }
}
