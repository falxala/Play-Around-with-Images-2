using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PlayAroundwithImages2.Converters
{
    public class RotateConverter : IValueConverter
    {
        //使用しない
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var width = (double)value;
            return width / 240;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
