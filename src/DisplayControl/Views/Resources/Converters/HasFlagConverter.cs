using System;
using System.Globalization;
using System.Windows.Data;

namespace DisplayControl.Views.Resources.Converters
{
    internal class HasFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int flags = (int)value;
            int comparisonFlag = (int)parameter;

            return (flags & comparisonFlag) != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}