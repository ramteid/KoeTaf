using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Patrick Vogt
    /// </summary>
    class UserAccountSearchComboBoxItemModel
    {
        public enum Type
        {
            Benutzername = 0,
            Password
        }

        public string Value { get; set; }

        public Type SearchType { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
