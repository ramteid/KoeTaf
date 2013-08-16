using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DB = KöTaf.DataModel;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Anotonios Fesenmeier
    /// Dient der Verarbeitung von Revenues in den Datagrids
    /// </summary>
    public class RevenueModel
    {
        public DateTime revStartDate { get; set; }
        public DateTime revEndDate {get; set;}

        public String revDescription {get; set;}

        public IEnumerable<CBItem> revType { get; set; }

        

        public String revAmount {get; set; }

        public Boolean isAdded { get; set; }

        public int revenueID { get; set; }

    }

    public class CBItem
    {
        public String Name { get; set; }
    }

}
