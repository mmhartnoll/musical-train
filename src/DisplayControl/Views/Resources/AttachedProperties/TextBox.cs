using System.Windows;

namespace DisplayControl.Views.Resources.AttachedProperties
{
    public class TextBox : DependencyObject
    {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty
            .RegisterAttached("Watermark", typeof(string), typeof(TextBox), new(null));

        public static void GetWatermark(DependencyObject d) => d.GetValue(WatermarkProperty);
        public static void SetWatermark(DependencyObject d, string value) => d.SetValue(WatermarkProperty, value);
    }
}