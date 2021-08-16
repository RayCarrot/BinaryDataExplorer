using System;
using System.Globalization;
using System.Windows;

namespace BinaryDataExplorer
{
    public class LevelToIndentConverter : BaseValueConverter<LevelToIndentConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness((int)value * 10, 0, 0, 0);
        }
    }
}