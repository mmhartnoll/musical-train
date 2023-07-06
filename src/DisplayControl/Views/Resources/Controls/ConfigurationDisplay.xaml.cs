using DomainModel.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DisplayControl.Views.Resources.Controls
{
    public partial class ConfigurationDisplay : UserControl, INotifyPropertyChanged
    {
        private double scaleFactor;
        private double offsetX;
        private double offsetY;

        public ObservableCollection<DisplayConfiguration> DisplayConfigurations
        {
            get => (ObservableCollection<DisplayConfiguration>)GetValue(DisplayConfigurationsProperty);
            set => SetValue(DisplayConfigurationsProperty, value);
        }

        public double ScaleFactor
        {
            get => scaleFactor;
            set => SetProperty(ref scaleFactor, value, nameof(ScaleFactor));
        }

        public double OffsetX
        {
            get => offsetX;
            set => SetProperty(ref offsetX, value, nameof(OffsetX));
        }

        public double OffsetY
        {
            get => offsetY;
            set => SetProperty(ref offsetY, value, nameof(OffsetY));
        }

        public static readonly DependencyProperty DisplayConfigurationsProperty =
            DependencyProperty.Register("DisplayConfigurations", typeof(ObservableCollection<DisplayConfiguration>), typeof(ConfigurationDisplay),
                new PropertyMetadata(default, new(OnDisplaysChanged)));

        public event PropertyChangedEventHandler? PropertyChanged;

        public ConfigurationDisplay()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            CalculateScaleAndOffsets();
        }

        private void CalculateScaleAndOffsets()
        {
            IEnumerable<DisplayConfiguration> displayConfigurations = DisplayConfigurations?.Cast<DisplayConfiguration>() ?? Enumerable.Empty<DisplayConfiguration>();

            if (displayConfigurations.Any())
            {
                int minPosX = displayConfigurations.Select(dp => dp.PositionX).Min();
                int maxPosX = displayConfigurations.Select(dp => dp.PositionX + (int)dp.ResolutionX).Max();
                int totalScreenWidth = maxPosX - minPosX;

                int minPosY = displayConfigurations.Select(dp => dp.PositionY).Min();
                int maxPosY = displayConfigurations.Select(dp => dp.PositionY + (int)dp.ResolutionY).Max();
                int totalScreenHeight = maxPosY - minPosY;

                ScaleFactor = Math.Min(ActualWidth / totalScreenWidth, ActualHeight / totalScreenHeight);

                OffsetX = ((ActualWidth - totalScreenWidth * scaleFactor) / 2) - (minPosX * scaleFactor);
                OffsetY = ((ActualHeight - totalScreenHeight * scaleFactor) / 2) - (minPosY * scaleFactor);
            }
            else
            {
                ScaleFactor = 1.0;
                OffsetX = 0;
                OffsetY = 0;
            }
        }

        private static void OnDisplaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => (d as ConfigurationDisplay)!.OnDisplaysChanged();

        private void OnDisplaysChanged()
            => CalculateScaleAndOffsets();

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
            => CalculateScaleAndOffsets();

        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        protected void SetProperty<T>(ref T field, T value, string propertyName, IEqualityComparer<T>? comparer = null)
        {
            ValidateProperty(value, propertyName);
            if ((comparer ?? EqualityComparer<T>.Default).Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        private void ValidateProperty<T>(T value, string propertyName)
        {
            ValidationContext context = new(this) { MemberName = propertyName };
            Validator.ValidateProperty(value, context);
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new InvalidOperationException($"Invalid propery name '{propertyName}'.");
        }
    }
}