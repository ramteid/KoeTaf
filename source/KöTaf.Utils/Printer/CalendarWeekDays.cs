/**
 * Class: CalendarWeekDays
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-05-05
 * 
 * Last modification: 2013-06-19 / Michael Müller
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace KöTaf.Utils.Printer
{
    /// <summary>
    /// Diese Klasse errechnet die Wochentage im Voraus
    /// Notwendig für den Berechtigungsausweis.
    /// </summary>
    public class CalendarWeekDays
    {
        /// <summary>
        /// Finde die derzeitige Kultur
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        /// 
        #region Contructor
        public CalendarWeekDays() { 
        
        }
        
        #endregion
        #region Methods
        public List<int> NumberOfWeek(DateTime dte)
        {
            List<int> calendarWeekList = new List<int>();
            while (calendarWeekList.Count < 6)
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;

                // derzeitiger Kalender
                Calendar calendar = currentCulture.Calendar;

                // Kalenderwoche
                int calendarWeek = calendar.GetWeekOfYear(dte,
                   currentCulture.DateTimeFormat.CalendarWeekRule,
                   currentCulture.DateTimeFormat.FirstDayOfWeek);

                // Überprüfe, falls Kalender Woche größer als 52 Kalenderwochen ist.
                // Und korrigiere Berechnung dementsprechend.
                if (calendarWeek > 52)
                {
                    dte = dte.AddDays(7);
                    int testCalendarWeek = calendar.GetWeekOfYear(dte,
                       currentCulture.DateTimeFormat.CalendarWeekRule,
                       currentCulture.DateTimeFormat.FirstDayOfWeek);
                    if (testCalendarWeek == 2)
                        calendarWeek = 1;
                }
                int WeekNr = calendarWeek;
                calendarWeekList.Add(WeekNr);
                TimeSpan t = new TimeSpan(7, 0, 0, 0);
                dte = dte.Add(t);
            }
            // Gebe Kalenderwochen als Liste zurück.
            return calendarWeekList;
        }
    }
}
#endregion