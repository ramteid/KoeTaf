using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse Kassenabschlussbeleg: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class CashClosureReport
    {
        /// <summary>
        /// Fügt einen neuen Kassenabschlussbeleg der Datenbank hinzu
        /// </summary>
        /// <param name="cashClosureID">Kassenabschluss ID</param>
        /// <param name="printDone">Wurde gedruckt</param>
        /// <param name="printUserAccountID">Druckuser</param>
        /// <param name="printDate">Druckdatum</param>
        /// <param name="done">Erledigt</param>
        /// <param name="doneDate">Erledigungsdatum</param>
        /// <param name="doneUserAccountID">Erledigungs User</param>
        /// <param name="revenues">Einnahmen</param>
        /// <param name="expenses">Ausgaben</param>
        /// <param name="sum">Summe</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int cashClosureID, bool printDone, DateTime? printDate, bool done,
            DateTime? doneDate, int? printUserAccountID = null, int? doneUserAccountID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var cashClosureReport = new CashClosureReport
                {
                    CashClosure = db.CashClosures.Single(cc => cc.CashClosureID == cashClosureID),
                    PrintDone = printDone,
                    PrintDate = printDate,
                    Done = done,
                    DoneDate = doneDate,
                };

                if (printUserAccountID.HasValue)
                {
                    cashClosureReport.PrintUserAccount = db.UserAccounts.Single(ua => ua.UserAccountID == printUserAccountID.Value);
                }

                if (doneUserAccountID.HasValue)
                    cashClosureReport.DoneUserAccount = db.UserAccounts.Single(ua => ua.UserAccountID == doneUserAccountID.Value);

                db.CashClosureReports.AddObject(cashClosureReport);
                db.SaveChanges();

                return cashClosureReport.CashClosureReportID;
            }
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Eintrag in der Datenbank
        /// </summary>
        /// <param name="cashClosureReportID"></param>
        /// <param name="cashClosureID">Kassenabschluss ID</param>
        /// <param name="printDone">Wurde gedruckt</param>
        /// <param name="printDate">Druckdatum</param>
        /// <param name="printUserAccountID">Druckuser</param>
        /// <param name="done"></param>
        /// <param name="doneDate"></param>
        /// <param name="doneUserAccountID"></param>
        /// <param name="revenues"></param>
        /// <param name="expenses"></param>
        /// <param name="sum"></param>
        public static void Update(int cashClosureReportID, int cashClosureID, bool printDone, DateTime printDate,
            int? printUserAccountID, bool done, DateTime? doneDate, int? doneUserAccountID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var cashClosureReport = db.CashClosureReports.Single(ccr => ccr.CashClosureReportID == cashClosureReportID);
                cashClosureReport.CashClosure = db.CashClosures.Single(cc => cc.CashClosureID == cashClosureID);
                cashClosureReport.PrintDone = printDone;
                cashClosureReport.PrintDate = printDate;
                cashClosureReport.PrintUserAccount = (printUserAccountID.HasValue) ? db.UserAccounts.Single(ua => ua.UserAccountID == printUserAccountID.Value) : null;
                cashClosureReport.Done = done;
                cashClosureReport.DoneDate = doneDate;
                cashClosureReport.DoneUserAccount = (doneUserAccountID.HasValue) ? db.UserAccounts.Single(ua => ua.UserAccountID == doneUserAccountID.Value) : null;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Eintrag aus der Datenbank
        /// </summary>
        /// <param name="cashClosureReportID"></param>
        public static void Delete(int cashClosureReportID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var report = db.CashClosureReports.Single(ccr => ccr.CashClosureReportID == cashClosureReportID);

                db.CashClosureReports.DeleteObject(report);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gibt eine Liste von Kassenabschlussbelegen zurück
        /// </summary>
        /// <param name="cashClosureReportID">Optionale Kassenbelegsnummer nach der gefiltert werden soll</param>
        /// <returns>Liste mit Kassenabschlussbelegen</returns>
        public static IEnumerable<CashClosureReport> GetCashClosureReports(int? cashClosureReportID = null, int? cashClosureID=null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var reports = db.CashClosureReports
                    .Include("CashClosure")
                    .Include("PrintUserAccount")
                    .Include("DoneUserAccount")
                    .AsQueryable();

                if (cashClosureReportID.HasValue)
                    reports = reports.Where(ccr => ccr.CashClosureReportID == cashClosureReportID.Value);

                if (cashClosureID.HasValue)
                    reports = reports.Where(ccr => ccr.CashClosure.CashClosureID == cashClosureID.Value);

                return reports.ToList();
            }
        }
    }
}
