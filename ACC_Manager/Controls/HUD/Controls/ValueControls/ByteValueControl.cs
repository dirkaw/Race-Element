﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ACC_Manager.Util.SystemExtensions;
using ACCManager.HUD.Overlay.Configuration;
using static ACCManager.HUD.Overlay.Configuration.OverlayConfiguration;

namespace ACCManager.Controls.HUD.Controls.ValueControls
{
    internal class ByteValueControl : IValueControl<byte>, IControl
    {
        private readonly Grid _grid;
        private readonly Label _label;
        private readonly Slider _slider;

        public FrameworkElement Control => _grid;
        public byte Value { get; set; }
        private readonly ConfigField _field;

        public ByteValueControl(ByteRangeAttribute byteRange, ConfigField configField)
        {
            _field = configField;
            _grid = new Grid()
            {
                Width = 220,
                Margin = new Thickness(0, 0, 7, 0),
                Background = new SolidColorBrush(Color.FromArgb(140, 2, 2, 2)),
                Cursor = Cursors.Hand
            };
            _grid.PreviewMouseLeftButtonUp += (s, e) => Save();
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });

            _label = new Label()
            {
                Content = Value,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            _grid.Children.Add(_label);
            Grid.SetColumn(_label, 0);

            _slider = new Slider()
            {
                Minimum = byteRange.Min,
                Maximum = byteRange.Max,
                TickFrequency = byteRange.Increment,
                IsSnapToTickEnabled = true,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 150
            };
            _slider.ValueChanged += (s, e) =>
            {
                _field.Value = _slider.Value.ToString();
                _label.Content = _slider.Value;
            };
            int value = int.Parse(configField.Value.ToString());
            value.Clip(byteRange.Min, byteRange.Max);
            _slider.Value = value;
            _grid.Children.Add(_slider);
            Grid.SetColumn(_slider, 1);
            _label.Content = _slider.Value;

            Control.MouseWheel += (sender, args) =>
            {
                int delta = args.Delta;
                _slider.Value += delta.Clip(-1, 1) * byteRange.Increment;
                args.Handled = true;
                Save();
            };
        }

        public void Save()
        {
            ConfigurationControls.SaveOverlayConfigField(_field);
        }
    }
}
