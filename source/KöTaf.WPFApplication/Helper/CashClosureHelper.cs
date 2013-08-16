using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.Utils.Parser;
using KöTaf.DataModel;
using KöTaf.DataModel.Enums;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Bietet Hilfsfunktionen für den Kassenabschluss
    /// </summary>
    class CashClosureHelper
    {
        public List<Account> closureAccounts { get; private set; }
        public List<Booking> bookings { get; private set; }

        /// <summary>
        /// Erzeugt Instanz und füllt Felder
        /// </summary>
        /// <param name="processOnlyBookingsFromCurrentClosure">Ob nur Buchungen des aktuellen Kassenschlusses verarbeitet werden sollen</param>
        public CashClosureHelper(bool processOnlyBookingsFromCurrentClosure = true)
        {
            this.closureAccounts = Account.GetAccounts().Where(a => a.IsOfficial && a.IsCapital).ToList();
            
            // Hole alle Buchungen, die nach dem letzten Kassenabschluss gebucht wurden
            DateTime lastCashClosure = BookingsHelper.getDateOfLastCashClosure();

            // Für Kassenabschluss filtere standardmäßig nur die Buchungen der akttuellen Periode
            if (processOnlyBookingsFromCurrentClosure)
                this.bookings = Booking.GetBookings().Where(b => b.Date > lastCashClosure && b.Date <= DateTime.Now).ToList();
            else
                this.bookings = Booking.GetBookings().ToList();
        }

        /// <summary>
        /// Hole alle Einkünfte aus angegebenem Zeitraum
        /// </summary>
        /// <param name="dateFrom">Zeitraum Start</param>
        /// <param name="dateTo">Zeitraum Ende</param>
        /// <returns></returns>
        public double getAllRevenuesForPeriod(DateTime dateFrom, DateTime dateTo)
        {
            IEnumerable<Booking> periodBookings = this.bookings.Where(b => dateFrom <= b.Date && b.Date <= dateTo);

            double revenues = periodBookings.Where(b => !(b.SourceAccount.IsOfficial && b.SourceAccount.IsCapital)).Sum(b => b.Amount);
            return revenues;
        }

        /// <summary>
        /// Hole alle Ausgaben aus angegebenem Zeitraum
        /// </summary>
        /// <param name="dateFrom">Zeitraum Start</param>
        /// <param name="dateTo">Zeitraum Ende</param>
        /// <returns></returns>
        public double getAllExpensesForPeriod(DateTime dateFrom, DateTime dateTo)
        {
            IEnumerable<Booking> periodBookings = this.bookings.Where(b => dateFrom <= b.Date && b.Date <= dateTo);

            double expenses = periodBookings.Where(b => !(b.TargetAccount.IsOfficial && b.TargetAccount.IsCapital)).Sum(b => b.Amount);
            return expenses;
        }

        /// <summary>
        /// Hole alle Einkünfte für ein Konto
        /// </summary>
        /// <param name="accountID">Konto-ID</param>
        /// <param name="dateFrom">Zeitraum Start</param>
        /// <param name="dateTo">Zeitraum Ende</param>
        /// <returns>Betrag</returns>
        public double getRevenuesForAccount(int accountID, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {
                List<Booking> revenueBookings = this.bookings.Where(b => b.TargetAccount.AccountID == accountID).ToList();

                if (dateFrom.HasValue && dateTo.HasValue)
                    revenueBookings = revenueBookings.Where(b => dateFrom <= b.Date && b.Date <= dateTo).ToList();

                double sumRevenues = 0.0;
                foreach (var revenueBooking in revenueBookings)
                    sumRevenues += revenueBooking.Amount;
                return sumRevenues;
            }
            catch
            {
                return -1.0;
            }
        }

        /// <summary>
        /// Hole alle Ausgaben für ein Konto
        /// </summary>
        /// <param name="accountID">Konto-ID</param>
        /// <param name="dateFrom">Zeitraum Start</param>
        /// <param name="dateTo">Zeitraum Ende</param>
        /// <returns>Betrag</returns>
        public double getExpensesForAccount(int accountID, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {
                List<Booking> expenseBookings = this.bookings.Where(b => b.SourceAccount.AccountID == accountID).ToList();

                if (dateFrom.HasValue && dateTo.HasValue)
                    expenseBookings = expenseBookings.Where(b => dateFrom <= b.Date && b.Date <= dateTo).ToList();

                double sumExpenses = 0.0;
                foreach (var expenseBooking in expenseBookings)
                    sumExpenses += expenseBooking.Amount;
                return sumExpenses;
            }
            catch
            {
                return -1.0;
            }
        }

        /// <summary>
        /// Neues Saldo = Altes Saldo + Einnahmen - Ausgaben
        /// </summary>
        /// <param name="accountID">Konto-ID</param>
        /// <returns>Betrag</returns>
        public double getNewBalanceForAccount(int accountID)
        {
            double oldBalance = getOldBalanceForAccount(accountID);
            double revenues = getRevenuesForAccount(accountID);
            double expenses = getExpensesForAccount(accountID);
            return oldBalance + revenues - expenses;
        }

        /// <summary>
        /// Hole das zugrundeliegende Saldo eines Kontos
        /// Nach Wechsel der Abrechnungsperiode wird beim Konto das alte Saldo / Grund-Saldo auf 0 gesetzt
        /// </summary>
        /// <param name="accountID">Konto-ID</param>
        /// <returns>Betrag</returns>
        public double getOldBalanceForAccount(int accountID)
        {
            try
            {
                Account account = this.closureAccounts.Where(a => a.AccountID == accountID).FirstOrDefault();

                // Bei Überschreitung des kontospezifischen Nullzeitraums wird die Bilanz/das Saldo auf 0 gesetzt
                if (exceededZeroPeriod(account.ZeroPeriodEnum))
                    return 0.0;
                else
                    return account.LatestBalance;
            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Prüfe ob Nullzeitraum überschritten wurde
        /// true: Periode überschritten
        /// </summary>
        /// <param name="period">Nullzeitraum</param>
        /// <returns>ob überschritten</returns>
        public static bool exceededZeroPeriod(ZeroPeriod period)
        {
            try
            {
                switch (period)
                {
                    case ZeroPeriod.EveryCashClosure:
                        return true;

                    case ZeroPeriod.Monthly:
                        int currentMonth = DateTime.Now.Month;
                        int lastClosureMonth = BookingsHelper.getDateOfLastCashClosure().Month;
                        return !(lastClosureMonth == currentMonth);

                    case ZeroPeriod.Annually:
                        int currentYear = DateTime.Now.Year;
                        int lastClosureYear = BookingsHelper.getDateOfLastCashClosure().Year;
                        return !(lastClosureYear == currentYear);

                    case ZeroPeriod.Never:
                        return false;

                    default:
                        return true;
                }
            }
            catch 
            {
                return true;
            }
        }
    }
}
