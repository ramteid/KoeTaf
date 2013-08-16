using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse Kassenabrechnung: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class CashClosureReceipt
    {
        #region Static Methods

        /// <summary>
        /// Fügt eine neue Kassenabrechnung der Datenbank hinzu
        /// </summary>
        /// <param name="cashClosureID">Die benötigte Kassenabschluss ID</param>
        /// <param name="printDone">Druck beendet</param>
        /// <param name="printDate">Druckdatum</param>
        /// <param name="printUserAccountID">Druck Benutzer</param>
        /// <param name="revenues">Einnahmen</param>
        /// <param name="expenses">Ausgaben</param>
        /// <param name="sum">Summe</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int cashClosureID, bool printDone, DateTime? printDate,
            int? printUserAccountID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var receipt = new CashClosureReceipt
                {
                    CashClosure = db.CashClosures.Single(cc => cc.CashClosureID == cashClosureID),
                    PrintDone = printDone,
                    PrintDate = printDate,
                    PrintUserAccount = (printUserAccountID.HasValue) ? db.UserAccounts.Single(ua => ua.UserAccountID == printUserAccountID.Value) : null,
                };

                db.CashClosureReceipts.AddObject(receipt);
                db.SaveChanges();

                return receipt.CashClosureReceiptID;
            }
        }

        /// <summary>
        /// Aktualisiert eine bestehende Kassenabrechnung in der Datenbank
        /// </summary>
        /// <param name="cashClosureReceiptID">Die benötigte Kassenabrechnungs ID</param>
        /// <param name="cashClosureID">Kassenabschluss ID</param>
        /// <param name="printDone">Druck erledigt</param>
        /// <param name="printDate">Druckdatum</param>
        /// <param name="printUserAccountID">Druck Benutzer</param>
        /// <param name="revenues">Einnahmen</param>
        /// <param name="expenses">Ausgaben</param>
        /// <param name="sum">Summe</param>
        public static void Update(int cashClosureReceiptID, int cashClosureID, bool printDone, DateTime? printDate,
            int? printUserAccountID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var receipt = db.CashClosureReceipts.Single(ccr => ccr.CashClosureReceiptID == cashClosureReceiptID);

                receipt.CashClosure = db.CashClosures.Single(cc => cc.CashClosureID == cashClosureID);
                receipt.PrintDone = printDone;
                receipt.PrintDate = printDate;
                receipt.PrintUserAccount = (printUserAccountID.HasValue) ? db.UserAccounts.Single(ua => ua.UserAccountID == printUserAccountID.Value) : null;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Eintrag aus der Datenbank
        /// </summary>
        /// <param name="cashClosureReceiptID"></param>
        public static void Delete(int cashClosureReceiptID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var receipt = db.CashClosureReceipts.Single(ccr => ccr.CashClosureReceiptID == cashClosureReceiptID);

                db.CashClosureReceipts.DeleteObject(receipt);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gibt eine Liste mit Kassenabrechnungen zurück
        /// </summary>
        /// <param name="cashClosureReceiptID">Optionale Filterung nach KassenabrechnungsID</param>
        /// <returns>Liste mit Kassenabrechnungen</returns>
        public static IEnumerable<CashClosureReceipt> GetCashClosureReceipts(int? cashClosureReceiptID = null, int? cashClosureID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var receipts = db.CashClosureReceipts
                    .Include("CashClosure")
                    .Include("PrintUserAccount")
                    .AsQueryable();

                if (cashClosureReceiptID.HasValue)
                    receipts = receipts.Where(r => r.CashClosureReceiptID == cashClosureReceiptID.Value);

                if (cashClosureID.HasValue)
                    receipts = receipts.Where(r => r.CashClosure.CashClosureID == cashClosureID.Value);

                return receipts.ToList();
            }
        }

        #endregion


    }
}
