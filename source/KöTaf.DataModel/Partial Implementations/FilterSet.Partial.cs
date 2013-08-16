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
    public partial class FilterSet
    {
        /// <summary>
        /// Gibt alle Filtersets zurück
        /// </summary>
        /// <param name="filterSetID">Kriterium zum filtern eines bestimmten Wertes</param>
        /// <param name="name">Name zum Filtern in der Datenbank</param>
        /// <returns>Liste aller Filtersets</returns>
        public static IEnumerable<FilterSet> GetFilterSets(int? filterSetID = null, int? formletterPatternID = null, string name = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var filterSets = db.FilterSets
                                    .Include("FormletterPattern")
                                    .Include("Filter")
                                    .AsQueryable();

                if (filterSetID.HasValue)
                    filterSets = filterSets.Where(fs => fs.FilterSetID == filterSetID.Value);

                if (formletterPatternID.HasValue)
                    filterSets = filterSets.Where(fs => fs.FormletterPattern.FormletterPatternID == formletterPatternID.Value);

                if (!string.IsNullOrEmpty(name))
                    filterSets = filterSets.Where(fs => fs.Name == name);

                return filterSets.ToList();
            }
        }

        /// <summary>
        /// Methode zum hinzufügen eines neuen Eintrags in der Datenbank
        /// </summary>
        /// <param name="filterID">Die ID des Filters</param>
        /// <param name="name">Name des Filters</param>
        /// <param name="linking">Linking</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(string name, string linking, int? formletterPatternID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var filterSet = new FilterSet()
                {
                    FormletterPattern = (formletterPatternID.HasValue) ? db.FormletterPatterns.Single(fp => fp.FormletterPatternID == formletterPatternID) : null,
                    Name = name,
                    Linking = linking
                };

                db.FilterSets.AddObject(filterSet);
                db.SaveChanges();

                return filterSet.FilterSetID;
            }
        }

        /// <summary>
        /// Methode zum löschen eines Eintrags in der Datenbank
        /// </summary>
        /// <param name="filterSetID"></param>
        public static void Delete(int filterSetID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var filterSet = db.FilterSets.Single(fs => fs.FilterSetID == filterSetID);

                db.FilterSets.DeleteObject(filterSet);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Der Name repräsentiert den String als Objekt
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
