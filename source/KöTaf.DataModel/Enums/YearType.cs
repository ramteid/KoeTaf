using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Enums
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Ein allgemeiner Wert nach welchem Jahr Kriterium gefiltert werden soll
    /// </summary>
    public enum YearType
    {
        /// <summary>
        /// Filterung nach dem jetzigen Jahr
        /// </summary>
        Current = 0,
        /// <summary>
        /// Filterung des vergangenen Jahres
        /// </summary>
        Past,
    }
}
