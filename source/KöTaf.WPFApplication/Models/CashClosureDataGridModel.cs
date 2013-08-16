using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.DataModel.Enums;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Datenmodell für das DataGrid, das die Konten beim Kassenabschluss anzeigt
    /// </summary>
    class CashClosureAccountsDataGridModel
    {
        public Account account { get; private set; }
        public string name { get; private set; }
        public string number { get; private set; }
        public string description { get; private set; }
        public string zeroPeriod { get; private set; }
        public string oldBalance { get; private set; }
        public string revenues { get; private set; }
        public string expenses { get; private set; }
        public string newBalance { get; private set; }
        public double newBalanceDOUBLE { get; private set; }

        /// <summary>
        /// Füllt die Felder
        /// </summary>
        /// <param name="account">Konto-Referenz</param>
        /// <param name="oldBalance">bisheriger Kontostand</param>
        /// <param name="revenues">Einnahmen</param>
        /// <param name="expenses">Ausgaben</param>
        /// <param name="newBalance">Neuer Kontostand</param>
        public CashClosureAccountsDataGridModel(Account account, double oldBalance, double revenues, double expenses, double newBalance)
        {
            this.account = account;
            this.name = SafeStringParser.safeParseToStr(account.Name);
            this.number = SafeStringParser.safeParseToStr(account.Number);
            this.description = SafeStringParser.safeParseToStr(account.Description);
            this.zeroPeriod = zeroPeriodToString(account.ZeroPeriodEnum);
            this.oldBalance = SafeStringParser.safeParseToMoney(oldBalance, true);
            this.revenues = SafeStringParser.safeParseToMoney(revenues, true);
            this.expenses = SafeStringParser.safeParseToMoney(expenses, true);
            this.newBalance = SafeStringParser.safeParseToMoney(newBalance, true);
            this.newBalanceDOUBLE = newBalance;
        }

        /// <summary>
        /// Wandelt den Nullzeitraum in einen lesbaren String um
        /// </summary>
        /// <param name="period">ZeroPeriod enum</param>
        /// <returns>text</returns>
        private string zeroPeriodToString(ZeroPeriod period)
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
