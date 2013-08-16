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
    public partial class UserAccount
    {
        /// <summary>
        /// Erstellt ein neues Benutzerkonto
        /// </summary>
        /// <param name="username">Benutzername</param>
        /// <param name="password">Passwort</param>
        /// <param name="isAdmin">Ob der Benutzer Administrator Rechte hat</param>
        /// <param name="imageName">Optionales Bild</param>
        /// <returns>Gibt die ID des Eintrags in der Datenbank zurück</returns>
        public static int Add(string username, string password, bool isAdmin, string imageName = null, bool isActive = true)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var userAccount = new UserAccount
                {
                    Username = username,
                    Password = password,
                    IsAdmin = isAdmin,
                    ImageName = imageName,
                    IsActive = isActive,
                };

                db.UserAccounts.AddObject(userAccount);
                db.SaveChanges();

                return userAccount.UserAccountID;
            }
        }

        /// <summary>
        /// Gibt eine Liste aller Benutzerkonten zurück
        /// </summary>
        /// <param name="userAccountId">Filterung nach Benutzerkonto ID</param>
        /// <param name="username">Filterung nach Benutzernamen</param>
        /// <param name="password">Filterung nach Passwort</param>
        /// <returns>Liste aller Benutzerkonten</returns>
        public static IEnumerable<UserAccount> GetUserAccounts(int? userAccountId = null, string username = null, string password = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var accounts = db.UserAccounts.AsQueryable();

                if (userAccountId.HasValue)
                    accounts = accounts.Where(a => a.UserAccountID == userAccountId.Value);

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    accounts = accounts.Where(a => a.Username == username && a.Password == password);

                return accounts.ToList();
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Eintrag aus der Datenbank
        /// </summary>
        /// <param name="userAccountID">Die benötigte BenutzerkontenID</param>
        public static void Delete(int userAccountID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var userAccount = db.UserAccounts.Single(u => u.UserAccountID == userAccountID);

                db.UserAccounts.DeleteObject(userAccount);
                db.SaveChanges();

            }
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Eintrag in der Datenbank
        /// </summary>
        /// <param name="userAccountID">Die benötigte Benutzerkonten ID</param>
        /// <param name="username">Benutzername</param>
        /// <param name="password">Password</param>
        /// <param name="isAdmin">Flag ob der Benutzer ein Admin ist</param>
        /// <param name="imageName">Optionaler Bildname</param>
        public static void Update(int userAccountID, string username, string password, bool isAdmin, 
            string imageName, bool isActive)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var userAccount = db.UserAccounts.Single(u => u.UserAccountID == userAccountID);
                userAccount.Username = username;
                userAccount.Password = password;
                userAccount.IsAdmin = isAdmin;
                userAccount.ImageName = imageName;
                userAccount.IsActive = isActive;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Author: Patrick Vogt
        /// Aktiviert ein Benutzerkonto
        /// </summary>
        /// <param name="userAccountID">Die benötigte Benutzerkonten-ID</param>
        public static void Activate(int userAccountID)
        {
            ToggleActivateState(userAccountID, true);
        }

        /// <summary>
        /// Author: Patrick Vogt
        /// Deaktiviert ein Benutzerkonto
        /// </summary>
        /// <param name="userAccountID">Die benötigte Benutzerkonten-ID</param>
        public static void Deactivate(int userAccountID)
        {
            ToggleActivateState(userAccountID, false);
        }

        /// <summary>
        /// Author: Patrick Vogt
        /// Wechselt zwischen aktivierten und deaktivierten Benutzerkonto
        /// </summary>
        /// <param name="userAccountID">Die benötigte Benutzerkonten-ID</param>
        /// <param name="activate">Benutzerkonto aktivieren oder deaktivieren</param>
        private static void ToggleActivateState(int userAccountID, bool activate)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var userAccount = db.UserAccounts.Single(u => u.UserAccountID == userAccountID);
                userAccount.IsActive = activate;
                db.SaveChanges();
            }
        }
    }
}
