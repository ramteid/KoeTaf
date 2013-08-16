using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    public class TeamSearchComboBoxItemModel
    {
        public enum Type
        {
            FullName = 0,
            Residence,
            TeamFunction
        }

        public string Value { get; set; }

        public Type SearchType { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
