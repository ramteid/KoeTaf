using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Antonios Fesenmeier
    /// 
    /// Dient der Verarbeitung von Childs in den Datagrids
    /// </summary>
    public class ChildModel
    {
        public int ChildID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public bool IsFemale { get; set; }

        public Boolean isAdded { get; set; }

        public IEnumerable<CBItem> genderType { get; set; }
    }
}
