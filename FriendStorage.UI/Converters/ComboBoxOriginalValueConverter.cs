using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using FriendStorage.UI.DataProvider.Lookups;

namespace FriendStorage.UI.Converters
{
    public class ComboBoxOriginalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var id = (int) value;
                var combobox = parameter as ComboBox;
                var lookupItem = combobox?.ItemsSource?.OfType<LookupItem>().Single(l => l.Id == id);
                if (lookupItem != null)
                    return lookupItem.DisplayValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}