using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Revenue
    {
        /// <summary>
        /// Gibt eine Liste von Einkommen zurück
        /// </summary>
        /// <param name="revenueID">Filterung nach Einkommen ID</param>
        /// <param name="personID">Filterung nach einer Person</param>
        /// <param name="startDate">Filterung nach Startdatum</param>
        /// <param name="endDate">Filterung nach Enddatum</param>
        /// <returns>Liste mit allen in der Datenbank gespeicherten Einkommen</returns>
        public static IEnumerable<Revenue> GetRevenues(int? revenueID = null, int? personID = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var revenues = db.Revenues
                                .Include("RevenueType")
                                .Include("Person")
                                .AsQueryable();

                if (revenueID.HasValue)
                    revenues = revenues.Where(r => r.RevenueID == revenueID.Value);

                if (personID.HasValue)
                    revenues = revenues.Where(r => r.Person.PersonID == personID.Value);

                if (startDate.HasValue)
                    revenues = revenues.Where(r => r.StartDate >= startDate.Value);

                if (endDate.HasValue)
                    revenues = revenues.Where(r => r.EndDate.HasValue && r.EndDate.Value <= endDate.Value);

                return revenues.ToList();
            }
        }

        /// <summary>
        /// Fügt einen Einkommenseintrag der Datenbank hinzu
        /// </summary>
        /// <param name="personID">Die benötigte Personen ID</param>
        /// <param name="revenueTypeID">Die Einkommensart ID</param>
        /// <param name="amount">Der Betrag</param>
        /// <param name="description">Optionale Beschreibung</param>
        /// <returns>Gibt die ID des Eintrags aus der Datenbank zurück</returns>
        public static int Add(int personID, int revenueTypeID, double amount, DateTime startDate, DateTime? endDate = null, string description = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var revenue = new Revenue
                {
                    Amount = amount,
                    Description = description,
                    RevenueType = db.RevenueTypes.Single(r => r.RevenueTypeID == revenueTypeID),
                    Person = db.Persons.Single(p => p.PersonID == personID),
                    StartDate = startDate,
                    EndDate = endDate
                };

                db.Revenues.AddObject(revenue);
                db.SaveChanges();

                return revenue.RevenueID;
            }
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Einkommen Eintrag in der Datenbank
        /// </summary>
        /// <param name="revenueID">Die benötigte Einkommens ID</param>
        /// <param name="revenueTypeID">Einkommensart ID</param>
        /// <param name="amount">Der Betrag</param>
        /// <param name="description">Eine Beschreibung</param>
        public static void Update(int revenueID, int revenueTypeID, double amount, string description, DateTime startDate, DateTime? endDate)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var revenue = db.Revenues.Single(r => r.RevenueID == revenueID);
                revenue.RevenueType = db.RevenueTypes.Single(r => r.RevenueTypeID == revenueTypeID);
                revenue.Amount = amount;
                revenue.Description = description;
                revenue.StartDate = startDate;
                revenue.EndDate = endDate;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Einkommen Eintrag aus der Datenbank
        /// </summary>
        /// <param name="revenueID">Die ID des Eintrags in der Datenbank</param>
        public static void Delete(int revenueID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var revenue = db.Revenues.Single(r => r.RevenueID == revenueID);

                db.Revenues.DeleteObject(revenue);
                db.SaveChanges();
            }
        }
    }
}
