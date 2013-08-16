using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    public class FormletterColumnCsvDocumentAssignment
    {
        public string csv_col_name { get; private set; }
        public FormletterTableAssignment formletterTableAssignment { get; private set; }

        public FormletterColumnCsvDocumentAssignment(string csv_col_name, FormletterTableAssignment formletterTableAssignment)
        {
            this.csv_col_name = csv_col_name;
            this.formletterTableAssignment = formletterTableAssignment;
        }

        public override string ToString()
        {
            return csv_col_name + " -> " + formletterTableAssignment;
        }
    }
}
