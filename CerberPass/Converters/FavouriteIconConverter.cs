using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows; // Poprawne dla WPF

namespace CerberPass.Converters
{
    public class FavouriteIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isFavourite = (bool)value;
            return isFavourite ? "Favorite" : "FavoriteOutline"; // Przykład: nazwy symboli mogą się różnić
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
