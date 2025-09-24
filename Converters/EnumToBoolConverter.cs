using System;
using System.Globalization;
using System.Windows.Data;

namespace renkimsumatin.Converters
{
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string enumValue = value.ToString() ?? string.Empty;
            string targetValue = parameter.ToString() ?? string.Empty;

            return enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // When the RadioButton (or similar) is checked, return the enum value represented by parameter.
            // Otherwise, do nothing to avoid nullability warnings and unintended updates.
            if (value is bool isChecked && isChecked && parameter != null)
            {
                var paramString = parameter.ToString();
                if (!string.IsNullOrWhiteSpace(paramString))
                {
                    return Enum.Parse(targetType, paramString, ignoreCase: true);
                }
            }

            return Binding.DoNothing;
        }
    }
}