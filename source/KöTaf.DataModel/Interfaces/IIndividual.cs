using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Interfaces
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Interface zum implementieren zusätzlicher Eigenschaften bei Personen
    /// </summary>
    public interface IIndividual
    {
       /// <summary>
       /// Gibt das Alter einer Person zurück
       /// </summary>
       int Age { get; }
       /// <summary>
       /// Gibt den vollständigen Namen einer Person zurück
       /// </summary>
       string FullName { get; }
    }
}
