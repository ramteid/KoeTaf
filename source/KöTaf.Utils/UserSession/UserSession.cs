using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;

namespace KöTaf.Utils.UserSession
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert eine Benutzer-Session
    /// Speichert Informationen zum aktuell angemeldeten Benutzer
    /// </summary>
    public class UserSession
    {
        public static int userAccountID { get; set; }
        public static string userName { get; set; }
        public static bool isAdmin { get; set; }

        // Da viel Code mit UserAccount-Objekten arbeitet, hier redundanterweise noch das UserAccount-Objekt:
        public static UserAccount userAccount { get; set; }
    }
}
