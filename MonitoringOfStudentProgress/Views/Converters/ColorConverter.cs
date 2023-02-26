using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace JournalOfStudents.Views.Converters {
    internal class ColorConverter: IValueConverter {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            if (value == null) return null;
            if (!targetType.IsAssignableTo(typeof(IBrush))) throw new NotSupportedException();
            if (value is int @i_v)
                return @i_v switch {
                    0 => new SolidColorBrush(Colors.Red),
                    1 => new SolidColorBrush(Colors.Yellow),
                    2 => new SolidColorBrush(Colors.Green),
                    _ => null,
                };
            if (value is String @s_v) {
                if (@s_v == "Error") return new SolidColorBrush(Colors.Aqua);
                if (!float.TryParse(@s_v, out float v)) throw new NotSupportedException();
                value = v;
            }
            if (value is float @f_v) {
                if (@f_v < 1) return new SolidColorBrush(Colors.Red);
                if (@f_v < 1.5) return new SolidColorBrush(Colors.Yellow);
                if (@f_v < -0.001 || @f_v > 2.001) return null;
                return new SolidColorBrush(Colors.Green);
            }
            throw new NotSupportedException();
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
