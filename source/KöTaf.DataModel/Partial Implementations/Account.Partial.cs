using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel.Enums;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Account
    {
        #region Static Methods

        /// <summary>
        /// Gibt alle Konten zurück
        /// </summary>
        /// <param name="accountID">Es wird nach Konto ID gefiltert</param>
        /// <param name="accountName">Es wird nach Konto Name gefiltert</param>
        /// <param name="accountNumber">Es wird nach Kontonummer gefiltert</param>
        /// <returns>Liste mit allen Konten</returns>
        public static IEnumerable<Account> GetAccounts(int? accountID = null, string accountName = null,
            int? accountNumber = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var accounts = db.Accounts
                                .Include("Booking")
                                .AsQueryable();

                if (accountID.HasValue)
                    accounts = accounts.Where(a => a.AccountID == accountID.Value);

                if (accountNumber.HasValue)
                    accounts = accounts.Where(a => a.Number == accountNumber.Value);

                if (!string.IsNullOrEmpty(accountName))
                    accounts = accounts.Where(a => a.Name == accountName);

                return accounts.ToList();
            }
        }

        /// <summary>
        /// Funktion zum löschen eines vorhandenen Kontos
        /// </summary>
        /// <param name="accountID">Datenbank ID des Accounts</param>
        public static void Delete(int accountID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var account = db.Accounts.Single(a => a.AccountID == accountID);

                db.Accounts.DeleteObject(account);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Methode zum aktualisieren von Konten
        /// </summary>
        /// <param name="accountId">Konto ID welcher benötigt wird</param>
        /// <param name="accountName">Kontoname</param>
        /// <param name="accountNumber">Kontonummer</param>
        /// <param name="image">Optionales Bild</param>
        /// <param name="description">Beschreibung</param>
        public static void Update(int accountId, string accountName, int accountNumber,
             ZeroPeriod zeroPeriod, bool isOfficial, string description,
            double latestBalance, bool isCapital,bool isFixed = false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var account = db.Accounts.Single(a => a.AccountID == accountId);
                account.Name = accountName;
                account.Number = accountNumber;
                account.Description = description;
                account.ZeroPeriod = (int)zeroPeriod;
                account.IsOfficial = isOfficial;
                account.LatestBalance = latestBalance;
                account.IsFixed = isFixed;
                account.IsCapital = isCapital;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Methode zum hinzufügen eines neuen Kontos
        /// </summary>
        /// <param name="name">Kontoname</param>
        /// <param name="accountNumber">Kontonummer</param>
        /// <param name="image">Bild</param>
        /// <param name="description">Optionale Beschreibung</param>
        /// <returns>Gibt den ID Wert des gespeicherten Eintrags in der Datenbank zurück</returns>
        public static int Add(string name, int accountNumber, bool isOfficial, ZeroPeriod zeroPeriod,
        bool isFixed,  double? latestBalance = null, string description = null, bool isCapital = false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var account = new Account()
                {
                    Name = name,
                    Number = accountNumber,
                    Description = description,
                    IsOfficial = isOfficial,
                    ZeroPeriod = (int)zeroPeriod,
                    IsFixed = isFixed,
                    LatestBalance = ((latestBalance.HasValue) ? latestBalance.Value : 0),
                    IsCapital=isCapital
                };

                db.Accounts.AddObject(account);
                db.SaveChanges();

                return account.AccountID;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gibt die Periode als Enumwert zurück
        /// </summary>
        public ZeroPeriod ZeroPeriodEnum
        {
            get
            {
                return (Enums.ZeroPeriod)this.ZeroPeriod;
            }
        }

        #endregion

    }
}
