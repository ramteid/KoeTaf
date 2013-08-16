using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KöTaf.WPFApplication.Converter
{
    /// <summary>
    /// Author: Antonios Fesenmeier
    /// </summary>
    public class RevenueConverter : IValueConverter
    {
        /// <summary>
        /// Nach dem Klicken wird die Image geändert
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            string deleteImage = "/Images/delete.png";
            string addedImage = "/Images/ok1.png";

            return (((bool)value) ? deleteImage : addedImage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}