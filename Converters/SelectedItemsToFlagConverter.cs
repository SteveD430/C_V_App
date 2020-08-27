using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace C_V_App.Converters
{

    [ValueConversion(typeof(object), typeof(string))]
    public class SelectedItemsToFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : ((ICollection)value).Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
