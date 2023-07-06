using System;
using System.Globalization;
using System.Windows.Data;

namespace DisplayControl.Views.Resources.Converters
{
    internal class VSyncFrequencyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            uint numerator = (uint)values[0];
            uint denominator = (uint)values[1];

            double value = (double)numerator / denominator;

            return $"{Math.Round(value)}Hz";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}