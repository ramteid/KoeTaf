using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Utils
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Klasse FormatUtils: Enthält Formatierungstools
    /// </summary>
    public class FormatUtils
    {
        /// <summary>
        /// Formatierung der vollständigen Adresse
        /// </summary>
        /// <param name="zipCode">Postleitzahl</param>
        /// <param name="city">Stadt</param>
        /// <param name="street">Strasse</param>
        /// <returns>Die vollständige formatierte Adresse</returns>
        public static string GetResidentialAddress(int zipCode, string city, string street)
        {
            return string.Format("{0} {1}, {2}", zipCode, city, street);
        }

        /// <summary>
        /// Alter
        /// </summary>
        /// <param name="birthday">Geburtsdatum</param>
        /// <returns>Das Alter</returns>
        public static int GetAge(DateTime birthday)
        {
            int years = (DateTime.Now.Year - birthday.Year);
            birthday = birthday.AddYears(years);
            if (DateTime.Now.CompareTo(birthday) < 0)
            {
                years--;
            }
            return years;
        }

        /// <summary>
        /// Gibt den vollständigen formatierten Namen zurück
        /// </summary>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachname</param>
        /// <returns>Den vollständigen formatierten Namen</returns>
        public static string GetFullName(string firstname, string lastname)
        {
            return string.Format("{0} {1}", lastname, firstname);
        }
    }
}
