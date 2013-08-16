using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Enums
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Ein Enum Wert wie bei der Statistik nach welchen Kriterien gefiltert werden und gezählt werden soll
    /// </summary>
    public enum CountChildGenderType
    {
        /// <summary>
        /// Filterung nach allen geschlechtern
        /// </summary>
        All = 0,
        /// <summary>
        /// Filterung nach weiblichen Personen
        /// </summary>
        Female,
        /// <summary>
        /// Filterung nach männlichen Personen
        /// </summary>
        Male,
    }
}
