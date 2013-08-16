using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert ein FilterSet mit allen zugehörigen Filtern
    /// </summary>
    public class FilterSetModel
    {
        public string linkingType { get; set; }
        public List<FilterModel> filterList { get; private set; }
        public string name { get; set; }

        /// <summary>
        /// Erzeuge ein neues Modell anhand des Filter-Typs
        /// </summary>
        /// <param name="linkingType">UND/ODER</param>
        public FilterSetModel(string linkingType)
        {
            if (linkingType == IniParser.GetSetting("FILTER", "defaultAndString") || linkingType == IniParser.GetSetting("FILTER", "defaultOrString"))
                this.linkingType = linkingType;
            else
                linkingType = "";
                //throw new Exception("FilterSetModel needs a valid linking type. Given: " + linkingType);

            filterList = new List<FilterModel>();
        }

        /// <summary>
        /// Fügt Filter anhand FilterModel hinzu
        /// </summary>
        /// <param name="filter">FilterModel</param>
        public void addFilter(FilterModel filter)
        {
            if (filter != null)
                this.filterList.Add(filter);
        }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(name))
                return IniParser.GetSetting("FILTER", "linkingType") + " " + linkingType.ToUpper() + 
                    ", " + IniParser.GetSetting("FILTER", "filter") + " " + filterList.Count;
            else
                return name;
        }
    }
}
