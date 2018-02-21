using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LazyMeter.Data;
using Color = System.Drawing.Color;

namespace LazyMeter
{
    public class ActivityTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (ActivityType) value;

            switch (type)
            {
                case ActivityType.Fun:
                    return new SolidColorBrush(Colors.BlueViolet);
                case ActivityType.Work:
                    return new SolidColorBrush(Colors.Bisque);
                case ActivityType.Univeristy:
                    return new SolidColorBrush(Colors.Chocolate);
                case ActivityType.Learning:
                    return new SolidColorBrush(Colors.SkyBlue);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
