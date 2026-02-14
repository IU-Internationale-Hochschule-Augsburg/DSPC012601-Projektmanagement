using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Projektmanagement_DesktopApp.Converters;

public class LoadColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int hours)
        {
            if (hours > 40) return Brushes.Red;
            if (hours > 30) return Brushes.Orange;
            return Brushes.Green;
        }
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
