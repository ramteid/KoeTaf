using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert ComboBox-Einträge bei den Buchungen und Kontensummen für die Kontenfilterung
    /// </summary>
    class AccountComboBoxModel
    {
        public Account account { get; private set; }
        public string accountName { get; private set; }
        public int accountNumber { get; private set; }

        /// <summary>
        /// Füllt Felder aus
        /// </summary>
        /// <param name="account">Konto</param>
        public AccountComboBoxModel(Account account)
        {
            this.account = account;
            this.accountName = account.Name;
            this.accountNumber = account.Number;
        }

        /// <summary>
        /// Spezialfall: Alle Eigenkapitalkonten
        /// </summary>
        public AccountComboBoxModel()
        {
            this.account = null;
            this.accountName = "Alle Eigenkapitalkonten";
            this.accountNumber = -1;
        }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            if (account != null)
                return SafeStringParser.safeParseToStr(this.accountName) + " (" + SafeStringParser.safeParseToStr(accountNumber) + ")";
            else
                return SafeStringParser.safeParseToStr(this.accountName);
        }
    }
}
