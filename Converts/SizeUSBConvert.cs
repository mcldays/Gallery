using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gallery.Converts
{
    class SizeUSBConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            double val = System.Convert.ToDouble(value);// Convert.ToDouble(vIn); value;
            return string.Format("{0} Gb", Math.Round(val / Math.Pow(1024d, 3d), 2));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
