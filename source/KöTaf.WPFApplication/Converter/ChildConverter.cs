using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KöTaf.WPFApplication.Converter
{
    /// <summary>
    /// Author: Antonios Fesenmeier, Georg Schmid
    /// </summary>
    public class ChildConverter : IValueConverter
    {
        /// <summary>
        /// Nach dem Klicken wird die Image geändert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value == true) ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}