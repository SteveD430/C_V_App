using System;
using System.Windows.Data;

namespace C_V_App.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool result;
            if (bool.TryParse(value.ToString(), out result))
            {
                return !result;
            }
            else
            {
                throw new Exception("Parameter must be boolean");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        #endregion
    }
}
