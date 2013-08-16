using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DB = KöTaf.DataModel;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    ///  Ermöglicht das befüllen von Comboboxen in Datagrids mit den RevenueTypes
    ///  Author: Antonios Fesenmeier
    /// </summary>
     class RevenueTypes : List<DB.RevenueType>
    {
        public RevenueTypes()
        {

            List<DB.RevenueType> list = new List<DB.RevenueType>();
            list = DB.RevenueType.GetRevenueTypes().ToList();

            foreach (DB.RevenueType type in list)
                this.Add(type);
        }
    }
}
