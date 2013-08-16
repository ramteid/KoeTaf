using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;
using KöTaf.Utils.UserSession;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Patrick Vogt
    /// Führt eine Korrekturbuchung durch
    /// </summary>
    class NegativeBooking
    {
        #region Static Methods
        public static int correct(Booking booking)
        {
            if (booking.IsCorrection)
                return -1;

            if (booking.Date > BookingsHelper.getDateOfLastCashClosure())
                return -2;

            int newSourceAccountID = booking.TargetAccount.AccountID;
            int newTargetAccountID = booking.SourceAccount.AccountID;
            double amount = booking.Amount;
            string description = "#" + booking.BookingID + " " + IniParser.GetSetting("ACCOUNTING", "negativeBooking") + 
                booking.SourceAccount.Number + " -> " +
                booking.TargetAccount.Number + ": " + amount + IniParser.GetSetting("APPSETTINGS", "currency");

            int bookingID = Booking.Add(newSourceAccountID, newTargetAccountID, amount, null, UserSession.userAccountID, description, true);
            if (bookingID < 0)
                return -3;

            return 0;
        }
        #endregion
    }
}
