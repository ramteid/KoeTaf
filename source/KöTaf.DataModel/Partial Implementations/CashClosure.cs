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
    public partial class CashClosure
    {
        #region Static Methods

        /// <summary>
        /// Fügt einen neuen Kassenabschluss der Datenbank hinzu
        /// </summary>
        /// <param name="cashClosureUserAccountID">Die UserAccount ID</param>
        /// <param name="closureDate">Datum des Kassenabschlusses</param>
        /// <param name="comment">Kommentar</param>
        /// <returns></returns>
        public static int Add(int cashClosureUserAccountID, DateTime closureDate, double revenue, double expense, string comment = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var cashClosure = new CashClosure
                {
                    ClosureDate = closureDate,
                    ClosureUserAccount = db.UserAccounts.Single(u => u.UserAccountID == cashClosureUserAccountID),
                    Comment = comment,
                    Sum = (revenue - expense),
                    Expense = expense,
                    Revenue = revenue
                };

                db.CashClosures.AddObject(cashClosure);
                db.SaveChanges();

                return cashClosure.CashClosureID;
            }
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Eintrag in der Datenbank
        /// </summary>
        /// <param name="cashClosureID"></param>
        /// <param name="closureUserAccountID"></param>
        /// <param name="closureDate"></param>
        /// <param name="comment"></param>
        public static void Update(int cashClosureID, int closureUserAccountID, DateTime closureDate, double revenue, double expense, string comment)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var cashClosure = db.CashClosures.Single(cc => cc.CashClosureID == cashClosureID);
                cashClosure.ClosureUserAccount = db.UserAccounts.Single(u => u.UserAccountID == closureUserAccountID);
                cashClosure.ClosureDate = closureDate;
                cashClosure.Comment = comment;
                cashClosure.Sum = (revenue - expense);
                cashClosure.Revenue = revenue;
                cashClosure.Expense = expense;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Eintrag aus der Datenbank
        /// </summary>
        /// <param name="cashClosureID">Die benötigte Kassenabschluss ID</param>
        public static void Delete(int cashClosureID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var cashClosure = db.CashClosures.Single(cc => cc.CashClosureID == cashClosureID);

                db.CashClosures.DeleteObject(cashClosure);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gibt eine Liste mit Kassenabschlüssen zurück
        /// </summary>
        /// <param name="cashClosureID">Optionale Filterung nach Kassenabschluss ID</param>
        /// <param name="closureUserAccountID">Optionale Filterung nach Benutzerkonto</param>
        /// <returns>Liste mit Kassenabschlüssen</returns>
        public static IEnumerable<CashClosure> GetCashClosures(int? cashClosureID = null, int? closureUserAccountID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var cashClosure = db.CashClosures
                    .Include("ClosureUserAccount")
                    .Include("CashClosureReport")
                    .Include("CashClosureReceipt")
                    .AsQueryable();

                if (cashClosureID.HasValue)
                    cashClosure = cashClosure.Where(cc => cc.CashClosureID == cashClosureID.Value);

                if (closureUserAccountID.HasValue)
                    cashClosure = cashClosure.Where(cc => cc.ClosureUserAccount.UserAccountID == closureUserAccountID.Value);

                return cashClosure.ToList();
            }
        }

        /// <summary>
        /// Gibt die Summe der Ein- oder Ausgabe zurück
        /// </summary>
        /// <param name="isRevenue">Ob es sich um eine Ein- oder Ausgabe handelt</param>
        /// <param name="type">Gegenwart oder Vergangenheit</param>
        /// <returns>Die Summe der Beträge</returns>
        public static double GetSum(bool isRevenue, YearType type = YearType.Current)
        {
            var date = DateTime.Now;
            if (type == YearType.Past)
            {
                date = date.AddYears(-1);
            }

            var startDate = new DateTime(date.Year, 1, 1);
            var endDate = new DateTime(date.Year, 12, 31);

            using (var db = new TafelModelContainer())
            {
                var cashClosures = db.CashClosures
                    .Where(cc => cc.ClosureDate >= startDate
                        && cc.ClosureDate <= endDate);
                if (cashClosures.Any())
                {
                    if (isRevenue)
                        return cashClosures.Sum(cc => cc.Revenue);
                    else
                        return cashClosures.Sum(cc => cc.Expense);
                }
                return 0;
            }
        }

        #endregion
    }
}
