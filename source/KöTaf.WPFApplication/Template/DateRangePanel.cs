using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KöTaf.WPFApplication.Views;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Definiert ein Panel, mit dem ein Anfangs- und ein Enddatum ausgewählt werden kann
    /// </summary>
    public class DateRangePanel
    {
        DatePicker datePickerFrom;
        DatePicker datePickerTo;
        private Action<DateTime> dateFromProcessingFunction;
        private Action<DateTime> dateToProcessingFunction;
        public WrapPanel panel { get; private set; }

        /// <summary>
        /// Definiert das DateRangePanel
        /// </summary>
        /// <param name="dateFromProcessingFunction">Funktion, die vom Start-DatePicker bei Änderung aufgerufen wird</param>
        /// <param name="dateToProcessingFunction">Funktion, die vom Ende-DatePicker bei Änderung aufgerufen wird</param>
        /// <param name="datePickerFrom">Referenz zum darzustellenden DatePicker für Start</param>
        /// <param name="datePickerTo">Referenz zum darzustellenden DatePicker für Ende</param>
        public DateRangePanel(Action<DateTime> dateFromProcessingFunction, Action<DateTime> dateToProcessingFunction, ref DatePicker datePickerFrom, ref DatePicker datePickerTo)
        {
            this.dateFromProcessingFunction = dateFromProcessingFunction;
            this.dateToProcessingFunction = dateToProcessingFunction;

            this.datePickerFrom = datePickerFrom;
            this.datePickerTo = datePickerTo;

            Label lbFrom = new Label();
            Label lbTo = new Label();
            lbFrom.Content = IniParser.GetSetting("APPSETTINGS", "dateRangeFrom");
            lbTo.Content = IniParser.GetSetting("APPSETTINGS", "dateRangeTo");
            lbTo.Margin = new Thickness(10, 0, 0, 0);

            this.datePickerFrom.Width = 95;
            this.datePickerTo.Width = 95;

            this.datePickerFrom.SelectedDate = BookingsHelper.getDateOfLastCashClosure();
            this.datePickerTo.SelectedDate = DateTime.Today;

            this.datePickerFrom.SelectedDateChanged += processDateFrom;
            this.datePickerTo.SelectedDateChanged += processDateTo;

            panel = new WrapPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Right;
            panel.VerticalAlignment = VerticalAlignment.Top;
            panel.Margin = new Thickness(30, 10, 20, 0);     // links nur 30px wegen Platzmangel in Toolbar von Modul pSums

            panel.Children.Add(lbFrom);
            panel.Children.Add(this.datePickerFrom);
            panel.Children.Add(lbTo);
            panel.Children.Add(this.datePickerTo);
        }

        /// <summary>
        /// Verarbeite geändertes Datum und rufe entsprechende Funktion auf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processDateFrom(object sender, RoutedEventArgs e)
        {
            if (datePickerFrom.SelectedDate == null)
                return;

            DateTime date = (DateTime)datePickerFrom.SelectedDate;
            if (this.dateFromProcessingFunction != null)
                dateFromProcessingFunction(date);
        }

        /// <summary>
        /// Verarbeite geändertes Datum und rufe entsprechende Funktion auf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processDateTo(object sender, RoutedEventArgs e)
        {
            if (datePickerTo.SelectedDate == null)
                return;

            DateTime date = (DateTime)datePickerTo.SelectedDate;
            if (this.dateToProcessingFunction != null)
                dateToProcessingFunction(date);
        }

    }

}
