using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DisplayControl.Views.Resources.Converters
{
    internal class PriorityErrorListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value != null && value != DependencyProperty.UnsetValue)
                    return value;
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}