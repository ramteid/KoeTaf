using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Enums
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Ein allgemeiner Wert nach welchen Kriterium beim zählen von Einträgen gefiltert werden soll
    /// </summary>
    public enum CountStateType
    {
        /// <summary>
        /// Filterung nach aktivierten und deaktivierten Personen
        /// </summary>
        All = 0,
        /// <summary>
        /// Filterung nach aktiven Personen
        /// </summary>
        Activated,
        /// <summary>
        /// Filterung nach deaktivierten Personen
        /// </summary>
        Deactivated
    }
}
