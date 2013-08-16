using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KöTaf.WPFApplication.Converter
{
    /// <summary>
    /// Author: Florian Wasielewski
    /// </summary>
    public class PersonStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string deactivateImage = "/Images/deactivate.png";
            string activateImage = "/Images/activate.png";

            return (((bool)value) ? activateImage : deactivateImage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
