using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert eine Feldzuordnung für die Spaltenzuordnungen im Serienbrief
    /// </summary>
    public class FormletterTableAssignment
    {
        public enum Groups : byte { Kunde, Sponsor, Mitarbeiter, Leer, 
                                    Datum };

        // Hier werden die anzuzeigenden möglichen Verknüpfungen definiert
        public enum Fields : byte { Anrede, Vorname, Nachname, Straße_Hsnr, PLZ, Wohnort, Einleitung, Nationalität, Geburtsland, Geburtsdatum, Gültigkeitsbeginn, Gültigkeitsende, Email, Telefon, Mobil, Letzter_Einkauf, Familienstand,
                                    Vorname_Partner, Nachname_Partner, Nationalität_Partner, Geburtsland_Partner, Telefon_Partner, Mobil_Partner, Email_Partner,
                                    Firmenname, Kontaktperson, Faxnummer,
                                    Teamfunktion, Leer,
                                    Datum
                                  };
        
        public Groups group { get; private set; }
        public Fields field { get; private set; }

        /// <summary>
        /// Erzeuge normale Verknüpfung mit Angabe von Gruppe und Feld
        /// </summary>
        /// <param name="group">Groups</param>
        /// <param name="field">Fields</param>
        public FormletterTableAssignment(Groups group, Fields field)
        {
            this.group = group;
            this.field = field;
        }

        /// <summary>
        /// Erzeuge Leereintrag
        /// </summary>
        public FormletterTableAssignment()
        {
            this.group = Groups.Leer;
            this.field = Fields.Leer;
        }

        /// <summary>
        /// Erzeuge eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.group == Groups.Leer && this.field == Fields.Leer)
                return "Leer";

            if (this.group.ToString().Equals(this.field.ToString()))
                return group.ToString().Replace("_", " ");
            else
                return (group + "." + field).Replace("_", " ");
        }
    }
}
