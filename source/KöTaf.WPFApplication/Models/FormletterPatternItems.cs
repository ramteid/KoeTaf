using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert in der Serienbrief-Verwaltung einen Datensatz in der Liste der verfügbaren Serienbriefe
    /// </summary>
    public class FormletterPatternItem
    {
        public string name { get; private set; }
        public int formletterPatternId { get; private set; }

        /// <summary>
        /// Speichert Name und formletter_pattern-ID
        /// </summary>
        /// <param name="name">Name der Serienbrief-Vorlage</param>
        /// <param name="formletterPatternId">ID der SB-Vorlage in der Datenbank</param>
        public FormletterPatternItem(string name, int formletterPatternId)
        {
            this.name = name;
            this.formletterPatternId = formletterPatternId;
        }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
