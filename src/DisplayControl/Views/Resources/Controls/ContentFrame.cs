using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DisplayControl.Views.Resources.Controls
{
    public class ContentFrame : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty
            .Register("Header", typeof(string), typeof(ContentFrame), new("Content Frame"));

        public static readonly DependencyProperty HeaderBackgroundBrushProperty = DependencyProperty
            .Register("HeaderBackgroundBrush", typeof(Brush), typeof(ContentFrame), new(Brushes.SteelBlue));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public Brush HeaderBackgroundBrush
        {
            get => (Brush)GetValue(HeaderBackgroundBrushProperty);
            set => SetValue(HeaderBackgroundBrushProperty, value);
        }

        static ContentFrame() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentFrame), new FrameworkPropertyMetadata(typeof(ContentFrame)));
    }
}