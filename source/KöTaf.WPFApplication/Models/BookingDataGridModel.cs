using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;
using System.Windows;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert das Datenmodell für das DataGrid der Buchungen
    /// </summary>
    class BookingDataGridModel
    {
        public Booking booking { get; private set; }
        public int bookingID { get; private set; }
        public string amount { get; private set; }
        public double amountDOUBLE { get; private set; }
        public DateTime date { get; private set; }
        public string dateString { get; private set; }
        public string description { get; private set; }
        public string sourceAccountString { get; private set; }
        public string targetAccountString { get; private set; }
        public string sourceAccountNumber { get; private set; }
        public string targetAccountNumber { get; private set; }
        public int sourceAccountNumberINT { get; private set; }
        public int targetAccountNumberINT { get; private set; }
        public string sourceAccountName { get; private set; }
        public string targetAccountName { get; private set; }
        public string user { get; private set; }
        public string customer { get; private set; }
        public bool sourceAccountIsOfficial { get; private set; }
        public bool targetAccountIsOfficial { get; private set; }
        public bool sourceAccountIsCapital { get; private set; }
        public bool targetAccountIsCapital { get; private set; }
        public Visibility visibilityCorrection { get; private set; }
        public Visibility visibilityDelete { get; private set; }
        public Visibility visibilityEdit { get; private set; }

        /// <summary>
        /// Lege neues Modell an
        /// </summary>
        /// <param name="booking">zugehörige Buchung</param>
        /// <param name="lastCashClosure">Datum letzter Kassenabschluss</param>
        public BookingDataGridModel(Booking booking, DateTime lastCashClosure)
        {
            this.booking = booking;
            this.bookingID = booking.BookingID;
            this.amount = SafeStringParser.safeParseToMoney(booking.Amount, true);
            this.amountDOUBLE = booking.Amount;
            this.date = booking.Date;
            this.dateString = SafeStringParser.safeParseToStr(booking.Date, true);
            this.description = SafeStringParser.safeParseToStr(booking.Description);
            this.sourceAccountString = SafeStringParser.safeParseToStr(booking.SourceAccount.Name) + " (" + SafeStringParser.safeParseToStr(booking.SourceAccount.Number) + ")";
            this.targetAccountString = SafeStringParser.safeParseToStr(booking.TargetAccount.Name) + " (" + SafeStringParser.safeParseToStr(booking.TargetAccount.Number) + ")";
            this.sourceAccountNumber = SafeStringParser.safeParseToStr(booking.SourceAccount.Number);
            this.targetAccountNumber = SafeStringParser.safeParseToStr(booking.TargetAccount.Number);
            this.sourceAccountNumberINT = booking.SourceAccount.Number;
            this.targetAccountNumberINT = booking.TargetAccount.Number; this.sourceAccountName = SafeStringParser.safeParseToStr(booking.SourceAccount.Name);
            this.targetAccountName = SafeStringParser.safeParseToStr(booking.TargetAccount.Name);
            this.sourceAccountIsOfficial = booking.SourceAccount.IsOfficial;
            this.targetAccountIsOfficial = booking.TargetAccount.IsOfficial;
            this.sourceAccountIsCapital = booking.SourceAccount.IsCapital;
            this.targetAccountIsCapital = booking.TargetAccount.IsCapital;

            if (booking.UserAccount != null)
                this.user = SafeStringParser.safeParseToStr(booking.UserAccount.Username);
            else
                this.user = "";

            if (booking.Person != null)
                this.customer = SafeStringParser.safeParseToStr(booking.Person.TableNo);
            else
                this.customer = "";

            // Korrekturbuchung-Button wird nur angezeigt wenn Datum vor dem letzten Kassenschluss
            if (booking.Date <= lastCashClosure)
                this.visibilityCorrection = Visibility.Visible;
            else
                this.visibilityCorrection = Visibility.Hidden;

            // Löschen/Edit-Button wird nur angezeigt wenn Datum nach dem letzten Kassenschluss
            if (booking.Date > lastCashClosure)
            {
                this.visibilityDelete = Visibility.Visible;
                this.visibilityEdit = Visibility.Visible;
            }
            else
            {
                this.visibilityDelete = Visibility.Hidden;
                this.visibilityEdit = Visibility.Hidden;
            }

            // Korrekturbuchungen sind nicht editierbar/korrigierbar
            if (booking.IsCorrection)
            {
                this.visibilityEdit = Visibility.Hidden;
                this.visibilityCorrection = Visibility.Hidden;
            }
        }
    }
}
