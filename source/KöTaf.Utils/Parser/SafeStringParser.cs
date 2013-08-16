using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace KöTaf.Utils.Parser
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Diese Klasse bietet Funktionen, um zahlreiche Datentypen absturzsicher zu String zu parsen
    /// Dabei gibt es nützliche Parameter um die resultierenden Strings zu formatieren
    /// </summary>
    public class SafeStringParser
    {
        /// <summary>
        /// parst int nullable zu string
        /// </summary>
        /// <param name="i">int oder null</param>
        /// <returns>string</returns>
        public static string safeParseToStr(int? i)
        {
            if (!(i.HasValue) || i == null)
                return "";
            else
                return ((int)i).ToString();
        }

        /// <summary>
        /// parst int zu string
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>string</returns>
        public static string safeParseToStr(int i)
        {
            return i.ToString();
        }

        /// <summary>
        /// parst Datum nullable zu string
        /// </summary>
        /// <param name="d">Datum oder null</param>
        /// <param name="includeTime">mit Zeit</param>
        /// <returns>string</returns>
        public static string safeParseToStr(DateTime? d, bool includeTime = false)
        {
            if (!(d.HasValue) || d == null)
                return "";
            else
                if (includeTime)
                    return ((DateTime)d).ToString("dd.MM.yyyy HH:mm");
                else
                    return ((DateTime)d).ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// parst Datum zu string
        /// </summary>
        /// <param name="d">Datum</param>
        /// <param name="includeTime">mit Zeit</param>
        /// <returns>string</returns>
        public static string safeParseToStr(DateTime d, bool includeTime = false)
        {
            if (d == null)
                return "";
            else
                if (includeTime)
                    return d.ToString("dd.MM.yyyy HH:mm");
                else
                    return d.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// parst string sicher zu string
        /// kürzt auf 4000 Zeichen für Datenbank
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>string</returns>
        public static string safeParseToStr(string s)
        {
            if (s == null)
                return "";
            else
                if (s.Length >= 4000)   // Maximale String-Länge in der SQL CE-Datenbank: 4000 Zeichen
                    s = s.Substring(0, 4000);
                return s;
        }

        /// <summary>
        /// parst double nullable zu string
        /// </summary>
        /// <param name="d">double oder null</param>
        /// <returns>string</returns>
        public static string safeParseToStr(double? d)
        {
            if (!(d.HasValue) || d == null)
                return "";
            else
                return d.ToString();
        }

        /// <summary>
        /// parst float nullable zu string
        /// </summary>
        /// <param name="f">float oder null</param>
        /// <returns>string</returns>
        public static string safeParseToStr(float? f)
        {
            if (!(f.HasValue) || f == null)
                return "";
            else
                return f.ToString();
        }

        /// <summary>
        /// parst bool nullable zu string
        /// </summary>
        /// <param name="b">bool oder null</param>
        /// <returns>string</returns>
        public static string safeParseToStr(bool? b)
        {
            if (!(b.HasValue) || b == null)
                return "";
            else
                if ((bool)b)
                    return IniParser.GetSetting("PARSING", "yes");
                else
                    return IniParser.GetSetting("PARSING", "no");
        }

        /// <summary>
        /// parst bool zu string
        /// </summary>
        /// <param name="b">bool</param>
        /// <returns>string</returns>
        public static string safeParseToStr(bool b)
        {
            if (b)
                return IniParser.GetSetting("PARSING", "yes");
            else
                return IniParser.GetSetting("PARSING", "no");
        }

        /// <summary>
        /// parst double nullable zu string mit Währungs-Formatierung
        /// </summary>
        /// <param name="d">double oder null</param>
        /// <param name="includeEuroSign">Euro-Zeichen anhängen</param>
        /// <returns>string</returns>
        public static string safeParseToMoney(double? d, bool includeEuroSign = false)
        {
            if (!(d.HasValue) || d == null)
                return "";
            else
            {
                CultureInfo culture = new CultureInfo("de-DE");
                string formatted = String.Format("{0:0.00}", d);

                if (includeEuroSign)
                    formatted += " €";

                return formatted;
            }
        }
    }
}
