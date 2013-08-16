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
    public partial class Booking
    {
        #region Static Methods

        /// <summary>
        /// Gibt eine Liste aller Buchungen zurück
        /// </summary>
        /// <param name="bookingID">ID zum filtern von Buchungen</param>
        /// <param name="userAccountID">Filterung nach Benutzerkonten</param>
        /// <returns>Liste aller Buchungseinträge</returns>
        public static IEnumerable<Booking> GetBookings(int? bookingID = null, int? userAccountID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var bookings = db.Bookings
                    .Include("UserAccount")
                     .Include("SourceAccount")
                     .Include("TargetAccount")
                     .Include("Person")
                    .AsQueryable();

                if (bookingID.HasValue)
                    bookings = bookings.Where(ar => ar.BookingID == bookingID.Value);

                if (userAccountID.HasValue)
                    bookings = bookings.Where(ar => ar.UserAccount.UserAccountID == userAccountID.Value);

                return bookings.ToList();
            }
        }

        /// <summary>
        /// Speichert einen neuen Eintrag in der Datenbank
        /// </summary>
        /// <param name="accountID">Konto ID welche benötigt wird</param>
        /// <param name="date">Datum des Eintrags</param>
        /// <param name="amount">Betrag des Eintrags</param>
        /// <param name="purchase"></param>
        /// <param name="srcAccountNumber">Quell Kontonummer</param>
        /// <param name="targetAccountNumber">Ziel Kontonummer</param>
        /// <param name="teamId">Optionale Team ID</param>
        /// <param name="sponsorId">Optionale Sponsor ID</param>
        /// <param name="description">Optionale Beschreibung</param>
        /// <param name="isRevenue">Boolean Wert ob es sich um ein Einkommen oder Ausgabe handelt</param>
        /// <returns>Gibt die ID des gespeicherten Eintrags in der Datenbank zurück</returns>
        public static int Add(int srcAccountID, int targetAccountID,
            double amount, int? personID = null, int? userAccountID = null, string description = null, bool isCorrection = false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var booking = new Booking()
                {
                    Date = DateTime.Now,
                    Description = description,
                    SourceAccount = db.Accounts.Single(a => a.AccountID == srcAccountID),
                    TargetAccount = db.Accounts.Single(a => a.AccountID == targetAccountID),
                    UserAccount = (userAccountID.HasValue) ? db.UserAccounts.Single(u => u.UserAccountID == userAccountID.Value) : null,
                    Amount = amount,
                    IsCorrection = isCorrection,
                    Person = (personID.HasValue) ? db.Persons.Single(p => p.PersonID == personID.Value) : null,
                };

                db.Bookings.AddObject(booking);
                db.SaveChanges();

                return booking.BookingID;
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Eintrag aus der Datenbank
        /// </summary>
        /// <param name="bookingId">Die ID des Eintrags in der Datenbank</param>
        public static void Delete(int bookingID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var booking = db.Bookings.Single(a => a.BookingID == bookingID);

                db.Bookings.DeleteObject(booking);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Methode zum aktualisieren eines vorhandenen Eintrags
        /// </summary>
        /// <param name="additionalRecordId">ID des Eintrags in der Datenbank</param>
        /// <param name="accountID">Konto ID in der Datenbank</param>
        /// <param name="date">Datum</param>
        /// <param name="amount">Betrag</param>
        /// <param name="purchase"></param>
        /// <param name="srcAccountNumber">Quell Kontonummer</param>
        /// <param name="targetAccountNumber">Ziel Kontonummer</param>
        /// <param name="description">Optionale Beschreibung</param>
        /// <param name="isRevenue">Boolean Wert ob es sich um eine Einnahme oder Ausgabe handelt</param>
        /// <param name="teamId">Optionale Team ID</param>
        /// <param name="sponsorID">Optionale Sponsor ID</param>
        public static void Update(int bookingID, int srcAccountID, int targetAccountID, int? userAccountID,
         int?personID,   double amount, string description, bool isCorrection = false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var booking = db.Bookings.Single(b => b.BookingID == bookingID);
                booking.SourceAccount = db.Accounts.Single(a => a.AccountID == srcAccountID);
                booking.TargetAccount = db.Accounts.Single(a => a.AccountID == targetAccountID);
                booking.UserAccount = (userAccountID.HasValue) ? db.UserAccounts.Single(u => u.UserAccountID == userAccountID.Value) : null;
                booking.Amount = amount;
                booking.Description = description;
                booking.IsCorrection = isCorrection;
                booking.Person = (personID.HasValue) ? db.Persons.Single(p => p.PersonID == personID.Value) : null;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gibt die Summe der Wertbeträge bei den Buchungen zurück
        /// </summary>
        /// <param name="isRevenue">Boolean Wert ob es sich um eine Ein- oder Ausgabe handelt</param>
        /// <param name="yearType">Typ ob es ein gegenwärtiger oder vergangener Wert ist</param>
        /// <returns>Summe aller Ein- oder Ausgaben</returns>
        public static double GetAmountSum(YearType yearType = YearType.Current)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                int year = (yearType == YearType.Current) ? DateTime.Now.Year : DateTime.Now.Year - 1;
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31);

                var additionalRecords = db.Bookings.Where(b => b.Date >= startDate
                                           && b.Date <= endDate);

                if (additionalRecords.Any())
                    return additionalRecords.Sum(ar => ar.Amount);
                return 0;
            }
        }

        #endregion
    }
}
