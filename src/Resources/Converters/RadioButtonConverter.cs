using System;
using System.Windows.Data;

namespace IAMHeimdall.Resources
{
    public class RadioButtonConverter : IValueConverter
    {
        #region Functions
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string)parameter == (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
        #endregion
    }
}
