using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert einen Filter für die Serienbrief/Listen-Filterauswahl
    /// </summary>
    public class FilterModel
    {
        // Definiert mögliche Felder
        public enum Groups : byte { Kunde, Sponsor, Mitarbeiter };
        public enum Criterions : byte { Vorname, Nachname, Straße_Hsnr, Hausnummer, PLZ, Wohnort, Geburtsdatum, Alter, Letzter_Einkauf, Anzahl_Kinder, Nationalität, Geburtsland, Gültigkeitsbeginn, Gültigkeitsende, Erfassungsdatum, Anzahl_Personen, Verheiratet,
                                        Firma, Kontaktperson, Serienbrief_erlaubt,
                                        Teamfunktion, Aktiv };
        public enum Operations : byte { gleich, ungleich, kleiner, größer };
        
        public Groups group { get; set; }
        public Criterions criterion { get; set; }
        public Operations operation { get; set; }
        public string value { get; set; }
        
        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IniParser.GetSetting("FILTER", "group") + " " + group +
                " / " + IniParser.GetSetting("FILTER", "criterion") + " " + criterion +
                " / " + IniParser.GetSetting("FILTER", "operation") + " " + operation +
                " / " + IniParser.GetSetting("FILTER", "value") + " " + value;
        }
    }
}
