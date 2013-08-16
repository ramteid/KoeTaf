using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Dient zur Auflistung einer Spaltenzuweisung von Serienbrief-Vorlagen
    /// </summary>
    public class FormletterColumnCsvDocumentAssignment
    {
        public string csv_col_name { get; private set; }
        public FormletterTableAssignment formletterTableAssignment { get; private set; }

        /// <summary>
        /// Erzeugt neue Spaltenzuweisung
        /// </summary>
        /// <param name="csv_col_name">Name der CSV-Spalte</param>
        /// <param name="formletterTableAssignment">Feldzuordnung für die Spaltenzuordnungen im Serienbrief</param>
        public FormletterColumnCsvDocumentAssignment(string csv_col_name, FormletterTableAssignment formletterTableAssignment)
        {
            this.csv_col_name = csv_col_name;
            this.formletterTableAssignment = formletterTableAssignment;
        }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return csv_col_name + " -> " + formletterTableAssignment;
        }
    }
}
