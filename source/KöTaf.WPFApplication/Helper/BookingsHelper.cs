using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using System.Globalization;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Bietet Hilfsfunktionen für Buchungen
    /// </summary>
    public class BookingsHelper
    {
        /// <summary>
        /// Hole das Datum des letzten Kassenabschlusses
        /// </summary>
        /// <returns>Datum</returns>
        public static DateTime getDateOfLastCashClosure()
        {
            // Hole Liste der Kassenabschlüsse, sortiert höchstes Datum an erster Stelle
            List<CashClosure> cashClosures = CashClosure.GetCashClosures().OrderBy(o => o.ClosureDate).Reverse().ToList();

            // Gebe Datum des letzten Kassenabschlusses zurück
            if (cashClosures.Count > 0)
            {
                CashClosure lastCashClosure = cashClosures[0];
                return lastCashClosure.ClosureDate;
            }
            else        // Wenn noch kein Kassenabschluss vorhanden, gebe Anfang des Tages zurück
            {
                return makeDateSmall(DateTime.Today);
            }
        }

        /// <summary>
        /// Gibt das übergebene Datum mit kleinster Stunden/Minuten/Sekundenzahl zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>kleines Datum</returns>
        public static DateTime makeDateSmall(DateTime date)
        {
            string sDate = String.Format("{0:d}", date);
            string format = sDate + " 00:00:00";

            IFormatProvider culture = new CultureInfo("de-DE", true);
            DateTime todayZero = DateTime.Parse(format, culture);
            return todayZero;
        }

        /// <summary>
        /// Gibt das übergebene Datum mit höchstmöglicher Stunden/Minuten/Sekundenzahl zurück
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>großes Datum</returns>
        public static DateTime makeDateGreat(DateTime date)
        {
            string sDate = String.Format("{0:d}", date);
            string format = sDate + " 23:59:59";

            IFormatProvider culture = new CultureInfo("de-DE", true);
            DateTime todayLatest = DateTime.Parse(format, culture);
            return todayLatest;
        }
    }
}
