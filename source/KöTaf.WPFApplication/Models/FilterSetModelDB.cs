using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Wird verwendet um anhand eines Datenbank-FilterSets ein FilterSetModel zu erzeugen
    /// </summary>
    public class FilterSetModelDB
    {
        /// <summary>
        /// Wandelt ein FilterSet der Datenbank in eine Instanz von FilterSetModel um
        /// </summary>
        /// <param name="filterSet">DataModel.FilterSet</param>
        /// <returns>FilterSetModel</returns>
        public static FilterSetModel getFilterSetModelFromFilterSet(FilterSet filterSet)
        {
            if (filterSet == null)
                return null;

            IEnumerable<Filter> filters = Filter.GetFilters(null, filterSet.FilterSetID);

            // Name des FilterSets
            FilterSetModel filterSetModel = new FilterSetModel(filterSet.Linking);
            filterSetModel.name = filterSet.Name;

            // füge diesem filterSet alle filter hinzu
            foreach (var filter in filters)
            {
                FilterModel filterModel = new FilterModel();
                // konvertiere die strings aus der datenbank in enums
                filterModel.group = (FilterModel.Groups)Enum.Parse(typeof(FilterModel.Groups), filter.Table);
                filterModel.criterion = (FilterModel.Criterions)Enum.Parse(typeof(FilterModel.Criterions), filter.Type);
                filterModel.operation = (FilterModel.Operations)Enum.Parse(typeof(FilterModel.Operations), filter.Operation);
                filterModel.value = SafeStringParser.safeParseToStr(filter.Value);
                filterSetModel.filterList.Add(filterModel);
            }
            return filterSetModel;
        }
    }
}
