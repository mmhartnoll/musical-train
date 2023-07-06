using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DisplayControl.Views.Resources.Converters
{
    internal class ScaleAndOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(value => value == DependencyProperty.UnsetValue))
                return 0;


            int valueToConvert = values[0] is uint ? 
                (int)(uint)values[0] : 
                (int)values[0];
            double scaleFactor = (double)values[1];

            double result = valueToConvert * scaleFactor;

            if (values.Length == 3)
            {
                double offset = (double)values[2];
                return result + offset;
            }
            else
                return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}