using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert ComboBox-Einträge für die Suche in Buchungen/Kontensummen
    /// </summary>
    class BookingSumsSearchComboBoxItemModel
    {
        public enum Type { Quellkontonummer, Zielkontonummer, Quellkontoname, Zielkontoname, Betrag, Beschreibung };
        public Type type { get; set; }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return type.ToString();
        }
    }
}
