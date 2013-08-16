using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel.Enums;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert den Wert für den Nullzeitraum in der ComboBox, der beim Anlegen/Ändern eines Kontos auswählbar ist
    /// </summary>
    class AccountZeroPeriodEnum
    {
        public ZeroPeriod period { get; private set; }

        public AccountZeroPeriodEnum(ZeroPeriod period)
        {
            this.period = period;
        }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            switch (period)
            {
                case ZeroPeriod.EveryCashClosure:
                    return IniParser.GetSetting("ZEROPERIODS", "everyCashClosure");

                case ZeroPeriod.Annually:
                    return IniParser.GetSetting("ZEROPERIODS", "annually");

                case ZeroPeriod.Monthly:
                    return IniParser.GetSetting("ZEROPERIODS", "monthly");

                case ZeroPeriod.Never:
                    return IniParser.GetSetting("ZEROPERIODS", "never");

                default:
                    return "";
            }
        }
    }
}
