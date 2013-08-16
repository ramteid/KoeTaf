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
    public partial class Filter
    {
        /// <summary>
        /// Gibt alle Filter zurück
        /// </summary>
        /// <param name="filterID">Nach welcher Filter ID gefiltert werden soll</param>
        /// <param name="table">Nach welcher Tabelle gefiltert werden soll</param>
        /// <returns>Liste aller Filter in der Datenbank</returns>
        public static IEnumerable<Filter> GetFilters(int? filterID = null, int? filterSetID = null, int? formletterPatternID = null, string table = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var filters = db.Filters
                                .Include("FilterSet")
                                .AsQueryable();

                if (filterID.HasValue)
                    filters = filters.Where(f => f.FilterID == filterID.Value);

                if (filterSetID.HasValue)
                    filters = filters.Where(f => f.FilterSet.FilterSetID == filterSetID.Value);

                if (!string.IsNullOrEmpty(table))
                    filters = filters.Where(f => f.Table == table);

                if (formletterPatternID.HasValue)
                    filters = filters.Where(f => f.FilterSet.FormletterPattern.FormletterPatternID == formletterPatternID.Value);

                return filters.ToList();
            }
        }

        /// <summary>
        /// Methode zum hinzufügen eines neuen Filters
        /// </summary>
        /// <param name="table">Tabelle des Filters</param>
        /// <param name="column">Spalte des Filters</param>
        /// <param name="type">Typ des Filters</param>
        /// <param name="operation">Operation des Filters</param>
        /// <param name="value">Wert des Filters</param>
        /// <returns>Die ID in der Datenbank</returns>
        public static int Add(int filtersetID, string table,  string type, string operation, string value)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var filter = new Filter()
                {
                    FilterSet = db.FilterSets.Single(fs => fs.FilterSetID == filtersetID),
                    Table = table,
                    Type = type,
                    Operation = operation,
                    Value = value
                };

                db.Filters.AddObject(filter);
                db.SaveChanges();

                return filter.FilterID;
            }
        }

        /// <summary>
        /// Methode zum löschen eines Filters in der Datenbank
        /// </summary>
        /// <param name="filterID">Die ID des Filters</param>
        public static void Delete(int filterID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var filter = db.Filters.Single(f => f.FilterID == filterID);

                db.Filters.DeleteObject(filter);
                db.SaveChanges();
            }
        }

    }
}
