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
    public partial class TeamFunction
    {
        /// <summary>
        /// Statische Liste mit allen Teamfunktionen
        /// </summary>
        private static IEnumerable<TeamFunction> _TeamFunctions;

        #region Static Functions

        /// <summary>
        /// Gibt alle Team Funktionen zurück
        /// </summary>
        /// <param name="teamFunctionID">Filterung nach Team Funktion ID</param>
        /// <returns>Liste aller Teamfunktionen</returns>
        public static IEnumerable<TeamFunction> GetTeamFunctions(int? teamFunctionID = null)
        {
            RefreshTeamFunctionList();

            var teamFunctions = _TeamFunctions;

            if (teamFunctionID.HasValue)
                teamFunctions = teamFunctions.Where(t => t.TeamFunctionID == teamFunctionID.Value);

            return teamFunctions.ToList();
        }

        /// <summary>
        /// Fügt eine neue Teamfunktion der Datenbank hinzu
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int Add(string name)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var teamFunction = new TeamFunction
                {
                    Name = name
                };


                db.TeamFunctions.AddObject(teamFunction);
                db.SaveChanges();

                return teamFunction.TeamFunctionID;
            }
        }

        /// <summary>
        /// Aktualisiert alle Teamfunktionen
        /// </summary>
        private static void RefreshTeamFunctionList()
        {
            if (_TeamFunctions == null)
            {
                using (TafelModelContainer db = new TafelModelContainer())
                {
                    _TeamFunctions = db.TeamFunctions.ToList();
                }
            }
        }

        #endregion
    }
}
