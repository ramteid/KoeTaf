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
    public partial class RevenueType
    {
        /// <summary>
        /// Statische Liste mit allen Einkommenstypen
        /// </summary>
        private static IEnumerable<RevenueType> _RevenueTypes;

        /// <summary>
        /// Gibt eine Liste aller Einkommensarten zurück
        /// </summary>
        /// <param name="revenueTypeID">Filterung nach einer Einkommensart</param>
        /// <returns>Liste aller Einkommensarten</returns>
        public static IEnumerable<RevenueType> GetRevenueTypes(int? revenueTypeID = null)
        {
            var revenueTypes = GetRevenueTypeList();

            if (revenueTypeID.HasValue)
                revenueTypes = revenueTypes.Where(rt => rt.RevenueTypeID == revenueTypeID.Value);

            return _RevenueTypes;
        }

        /// <summary>
        /// Statische Methode zum holen von Einkommensarten -> Singleton Pattern um Datenbank nicht permanent zu konnektieren
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<RevenueType> GetRevenueTypeList()
        {
            if (_RevenueTypes == null)
            {
                using (TafelModelContainer db = new TafelModelContainer())
                {
                    _RevenueTypes = db.RevenueTypes.ToList();
                }
            }
            return _RevenueTypes;
        }

        /// <summary>
        /// Methode zum einfügen einer neuen Einkommensart
        /// </summary>
        /// <param name="name">Der Name der Einkommensart</param>
        /// <returns>Der ID des Eintrags in der Datenbank</returns>
        public static int Add(string name)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var revenueType = new RevenueType
                {
                    Name = name
                };

                db.RevenueTypes.AddObject(revenueType);
                db.SaveChanges();

                // Liste aktualisieren
                _RevenueTypes = null;
                GetRevenueTypeList();

                return revenueType.RevenueTypeID;
            }
        }
    }
}
