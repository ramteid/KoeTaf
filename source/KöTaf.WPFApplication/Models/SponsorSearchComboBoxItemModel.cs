using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    public class SponsorSearchComboBoxItemModel
    {
        public enum Type
        {
            Nothing = 0,
            Name,
            Residence,
            FundingType,
            CompanyName,
        }

        public string Value { get; set; }

        public Type SearchType { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
