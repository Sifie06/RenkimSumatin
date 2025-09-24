using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace renkimsumatin.Utilities
{
    public static class WatermarkHelper
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkHelper),
                new PropertyMetadata(string.Empty, OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (e.NewValue != null)
                {
                    textBox.GotFocus += RemoveWatermark;
                    textBox.LostFocus += ShowWatermark;

                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        ShowWatermark(textBox, null);
                    }
                }
                else
                {
                    textBox.GotFocus -= RemoveWatermark;
                    textBox.LostFocus -= ShowWatermark;
                }
            }
            else if (d is ComboBox comboBox)
            {
                if (e.NewValue != null)
                {
                    comboBox.GotFocus += RemoveWatermarkCombo;
                    comboBox.LostFocus += ShowWatermarkCombo;

                    if (comboBox.SelectedItem == null && string.IsNullOrEmpty(comboBox.Text))
                    {
                        ShowWatermarkCombo(comboBox, null);
                    }
                }
                else
                {
                    comboBox.GotFocus -= RemoveWatermarkCombo;
                    comboBox.LostFocus -= ShowWatermarkCombo;
                }
            }
        }

        private static void RemoveWatermark(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && GetWatermark(textBox) == textBox.Text)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private static void ShowWatermark(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = GetWatermark(textBox);
                textBox.Foreground = Brushes.Gray;
            }
        }

        private static void RemoveWatermarkCombo(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && GetWatermark(comboBox) == comboBox.Text)
            {
                comboBox.Text = string.Empty;
                comboBox.Foreground = Brushes.Black;
            }
        }

        private static void ShowWatermarkCombo(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox && string.IsNullOrEmpty(comboBox.Text) && comboBox.SelectedItem == null)
            {
                comboBox.Text = GetWatermark(comboBox);
                comboBox.Foreground = Brushes.Gray;
            }
        }
    }
}