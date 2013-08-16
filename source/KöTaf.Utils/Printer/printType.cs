/**
 * Class: PrintType
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-05-2
 * 
 * Last modification: 2013-06-16 / Michael Müller
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.Utils.Printer
{
    /// <summary>
    /// Auswahl der Druckmöglichkeiten für Datagrids
    /// </summary>
    public enum PrintType
    {
        Sponsor = 0,
        Team,
        Client,
        Statistic,
        LopOffList
    }
}
