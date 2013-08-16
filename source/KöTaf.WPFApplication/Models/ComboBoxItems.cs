using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    public class ComboBoxItems
    {
        public string enumKey { get; private set; }
        public byte enumVal { get; private set; }

        public ComboBoxItems(string enumKey, byte enumVal)
        {
            this.enumKey = enumKey;
            this.enumVal = enumVal;
        }

        public override string ToString()
        {
            return this.enumKey.Replace("_", " ");
        }
    }
}
