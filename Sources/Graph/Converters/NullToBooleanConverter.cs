using System;
using System.Globalization;
using System.Windows.Data;

namespace SPR.Graph.Converters
{
    /*
     * Return true si l'objet est null
     */
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null)
                return false;
            else
                return true;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
