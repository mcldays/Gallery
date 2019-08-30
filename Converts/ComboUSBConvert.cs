using Gallery.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gallery.Converts
{
    class ComboUSBConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            
            if (value is USBModel)
            {
                USBModel model = (USBModel)value;
                return string.Format("{0} {1} {2} Gb", model.VolumeLabel.Replace(@"\", ""), model.Name, Math.Round(model.Size / Math.Pow(1024d, 3d)), 2);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
