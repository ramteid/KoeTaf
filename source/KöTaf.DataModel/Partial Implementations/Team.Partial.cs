using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel.Enums;
using KöTaf.DataModel.Utils;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Team
    {
        /// <summary>
        /// Gibt eine Liste von Teammitgliedern aus der Datenbank zurück
        /// </summary>
        /// <param name="teamID">Filterung nach einem Teammitglied</param>
        /// <returns>Liste aller Teammitglieder</returns>
        public static IEnumerable<Team> GetTeams(int? teamID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var teams = db.Teams
                    .Include("TeamFunction")
                    .Include("Title")
                    .AsQueryable();

                if (teamID.HasValue)
                    teams = teams.Where(t => t.TeamID == teamID.Value);

                return teams
                    .OrderBy(t=>t.LastName)
                    .ThenBy(t=>t.FirstName)
                    .ToList();
            }
        }

        /// <summary>
        /// Aktiviert ein Teammitglied
        /// </summary>
        /// <param name="teamID">Die benötigte ID des Teammitglieds</param>
        public static void Activate(int teamID)
        {
            ToggleActivateState(teamID, true);
        }

        /// <summary>
        /// Deaktiviert ein Teammitglied
        /// </summary>
        /// <param name="teamId">Die benötigte ID des Teammitglieds</param>
        public static void Deactivate(int teamId)
        {
            ToggleActivateState(teamId, false);
        }

        /// <summary>
        /// Schaltet zwischen verschiedenen Aktivierungsstati eines Teammitglieds um
        /// </summary>
        /// <param name="teamId">Die benötigte ID des Teammitglieds</param>
        /// <param name="activate">Teammitglied Aktívieren oder Deaktivieren</param>
        private static void ToggleActivateState(int teamId, bool activate)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var team = db.Teams.Single(t => t.TeamID == teamId);
                team.IsActive = activate;

                db.SaveChanges();
                db.AcceptAllChanges();
            }
        }

        /// <summary>
        /// Fügt ein neues Teammitglied der Datenbank hinzu
        /// </summary>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachnamen</param>
        /// <param name="street">Strasse</param>
        /// <param name="zipcode">Postleitzahl</param>
        /// <param name="city">Stadt</param>
        /// <param name="mobileNo">Handynummer</param>
        /// <param name="phoneNo">Telefonnummer</param>
        /// <param name="commercialPhoneNo">Telefonnummer Arbeit</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int titleID, int teamFunctionID, string firstname, string lastname, string street, int zipcode, string city, DateTime dateOfBirth, string mobileNo = null, string phoneNo = null, string email = null, bool isFormLetterAllowed = false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var title = db.Titles.Single(t => t.TitleID == titleID);
                var teamFunction = db.TeamFunctions.Single(t => t.TeamFunctionID == teamFunctionID);

                Team team = new Team()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    IsActive = true,
                    Street = street,
                    ZipCode = zipcode,
                    City = city,
                    MobileNo = mobileNo,
                    DateOfBirth = dateOfBirth,
                    IsFormLetterAllowed = isFormLetterAllowed,
                    PhoneNo = phoneNo,
                    Title = title,
                    TeamFunction = teamFunction,
                    Email = email,
                };

                db.Teams.AddObject(team);
                db.SaveChanges();

                return team.TeamID;
            }
        }

        /// <summary>
        /// Aktualisiert ein bestehendes Teammitglied
        /// </summary>
        /// <param name="teamId">Die benötigte ID des Teammitglieds</param>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachnamen</param>
        /// <param name="street">Strasse</param>
        /// <param name="zipcode">Postleitzahl</param>
        /// <param name="city">Stadt</param>
        /// <param name="mobileNo">Handynummer</param>
        /// <param name="phoneNo">Telefonnummer</param>
        /// <param name="commercialPhoneNo">Telefonnummer Arbeit</param>
        public static void Update(int teamId, DateTime dateOfBirth, int titleId, int teamFunctionId, string firstname, string lastname, string street, int zipcode, string city, string mobileNo, string phoneNo, string email, bool isFormLetterAllowed)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var team = db.Teams.Single(t => t.TeamID == teamId);
                team.FirstName = firstname;
                team.LastName = lastname;
                team.Street = street;
                team.ZipCode = zipcode;
                team.City = city;
                team.MobileNo = mobileNo;
                team.DateOfBirth = dateOfBirth;
                team.PhoneNo = phoneNo;
                team.Title = db.Titles.Single(t => t.TitleID == titleId);
                team.TeamFunction = db.TeamFunctions.Single(t => t.TeamFunctionID == teamFunctionId);
                team.Email = email;
                team.IsFormLetterAllowed = isFormLetterAllowed;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Zählt die Teammitglieder in der Datenbank
        /// </summary>
        /// <param name="type">Filterung nach Typ</param>
        /// <returns>Die Anzahl der Teammitglieder in der Datenbank</returns>
        public static int Count(CountStateType type = CountStateType.All)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var teams = db.Teams.AsQueryable();

                if (type != CountStateType.All)
                {
                    bool activateState = (type == CountStateType.Activated) ? true : false;
                    teams = teams.Where(t => t.IsActive == activateState);
                }

                return teams.Count();
            }
        }

        #region Properties

        /// <summary>
        /// Gibt den vollständigen Namen des Teammitglieds zurück
        /// </summary>
        public string FullName
        {
            get
            {
                return FormatUtils.GetFullName(FirstName, LastName);
            }
        }

        /// <summary>
        /// Gibt die vollständige Adresse des Teammitglieds zurück
        /// </summary>
        public string ResidentialAddress
        {
            get
            {
                return FormatUtils.GetResidentialAddress(ZipCode, City, Street);
            }
        }

        /// <summary>
        /// Gibt das Alter des Teammitglieds zurück
        /// </summary>
        public int Age
        {
            get
            {
                return FormatUtils.GetAge(DateOfBirth);
            }
        }

        #endregion
    }
}
