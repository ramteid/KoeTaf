using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Enums
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Enum: ZeroPeriod
    /// </summary>
    public enum ZeroPeriod
    {
        /// <summary>
        /// Jeden Kassenabschluss
        /// </summary>
        EveryCashClosure=0,
        /// <summary>
        /// Monatlich
        /// </summary>
        Monthly,
        /// <summary>
        /// Jährlich
        /// </summary>
        Annually,
        /// <summary>
        /// Niemals
        /// </summary>
        Never,
    }
}
