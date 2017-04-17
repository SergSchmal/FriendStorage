using System;
using System.Globalization;
using System.Windows.Data;

namespace FriendStorage.UI.Converters
{
    public class DatePickerOriginalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime?) value;
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToShortDateString();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}