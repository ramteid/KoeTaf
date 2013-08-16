using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.Utils.Parser;
using KöTaf.DataModel;
using KöTaf.DataModel.Enums;
using System.Windows;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Datenmodell für das DataGrid in der Kontenverwaltung
    /// </summary>
    class AccountManagerDataGridModel
    {
        public int accountID { get; private set; }
        public string accountName { get; private set; }
        public string accountNumber { get; private set; }
        public string description { get; private set; }
        public string isOfficial { get; private set; }
        public string isCapital { get; private set; }
        public string zeroPeriod { get; private set; }
        public string image { get; set; }
        public string isFixed { get; set; }
        public Visibility visibilityEdit { get; set; }
        public Visibility visibilityDelete { get; set; }

        /// <summary>
        /// Erzeugt neues Datenmodell für ein Konto
        /// </summary>
        /// <param name="account">Konto</param>
        public AccountManagerDataGridModel(Account account)
        {
            this.accountID = account.AccountID;
            this.accountName = SafeStringParser.safeParseToStr(account.Name);
            this.accountNumber = SafeStringParser.safeParseToStr(account.Number);
            this.description = SafeStringParser.safeParseToStr(account.Description);
            this.isOfficial = SafeStringParser.safeParseToStr(account.IsOfficial);
            this.isCapital = SafeStringParser.safeParseToStr(account.IsCapital);

            switch (account.ZeroPeriodEnum)
            {
                case ZeroPeriod.EveryCashClosure:
                    this.zeroPeriod = IniParser.GetSetting("ZEROPERIODS", "everyCashClosure");
                    break;

                case ZeroPeriod.Annually:
                    this.zeroPeriod = IniParser.GetSetting("ZEROPERIODS", "annually");
                    break;

                case ZeroPeriod.Monthly:
                    this.zeroPeriod = IniParser.GetSetting("ZEROPERIODS", "monthly");
                    break;

                case ZeroPeriod.Never:
                    this.zeroPeriod = IniParser.GetSetting("ZEROPERIODS", "never");
                    break;

                default:
                    this.zeroPeriod = "";
                    break;
            }

            if (account.IsFixed)
            {
                this.isFixed = IniParser.GetSetting("PARSING", "yes");
                visibilityEdit = Visibility.Hidden;
                visibilityDelete = Visibility.Hidden;
            }
            else
            {
                this.isFixed = IniParser.GetSetting("PARSING", "no");
                visibilityEdit = Visibility.Visible;
                visibilityDelete = Visibility.Visible;
            }
        }
    }
}
