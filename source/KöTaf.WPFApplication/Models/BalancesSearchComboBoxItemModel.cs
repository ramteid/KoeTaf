using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    class BalancesSearchComboBoxItemModel
    {
        public enum Type { Kontoname, Kontonummer };
        public Type type { get; set; }

        public override string ToString()
        {
            return type.ToString();
        }
    }
}
