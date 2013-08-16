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
    public partial class Title
    {
        /// <summary>
        /// Statische Liste mit allen Titel
        /// </summary>
        private static IEnumerable<Title> _Titles;

        #region Static Methods

        /// <summary>
        /// Gibt eine Liste aller Titel zurück
        /// </summary>
        /// <param name="titleID">Filterung nach Titel ID</param>
        /// <returns>Liste aller Titel</returns>
        public static IEnumerable<Title> GetTitles(int? titleID = null)
        {
            var titles = GetTitleList();

            if (titleID.HasValue)
                titles = _Titles.Where(t => t.TitleID == titleID);

            return titles.ToList();
        }

        /// <summary>
        /// Fügt einen neuen Titel der Datenbank hinzu
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int Add(string name)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var title = new Title
                {
                    Name=name
                };

                db.Titles.AddObject(title);
                db.SaveChanges();

                return title.TitleID;
            }
        }

        /// <summary>
        /// Methode zum holen der Titel -> Singleton Pattern um häufiges konnektieren zur Datenbank zu vermeiden
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Title> GetTitleList()
        {
            if (_Titles == null)
            {
                using (TafelModelContainer db = new TafelModelContainer())
                {
                    _Titles = db.Titles.ToList();
                }
            }
            return _Titles;
        }

        #endregion
    }
}
