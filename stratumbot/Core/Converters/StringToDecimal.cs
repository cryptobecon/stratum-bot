using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace stratumbot.Core.Converters
{
    class StringToDecimal : IValueConverter
    {
        // decimal -> string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        // string -> decimal
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((string)value != "" && value != null)
                return Conv.dec((string)value, true);
            return 0;
        }
    }
}
