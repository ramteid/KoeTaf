using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class FamilyState
    {
        /// <summary>
        /// Enthält eine Liste mit allen Stati
        /// </summary>
        private static IEnumerable<FamilyState> _FamilyStates;

        #region Constants

        /// <summary>
        /// Ledig
        /// </summary>
        public const string FAMILYSTATE_TYPE_LD = "LD";
        /// <summary>
        /// Verheiratet
        /// </summary>
        public const string FAMILYSTATE_TYPE_VH = "VH";
        /// <summary>
        /// Getrennt lebend
        /// </summary>
        public const string FAMILYSTATE_TYPE_GT = "GT";
        /// <summary>
        /// Geschieden
        /// </summary>
        public const string FAMILYSTATE_TYPE_GS = "GS";
        /// <summary>
        /// Verwitwet
        /// </summary>
        public const string FAMILYSTATE_TYPE_VW = "VW";
        /// <summary>
        /// Lebenspartnerschaft
        /// </summary>
        public const string FAMILYSTATE_TYPE_LP = "LP";
        /// <summary>
        /// Lebenspartnerschaft aufgehoben
        /// </summary>
        public const string FAMILYSTATE_TYPE_LA = "LA";
        /// <summary>
        /// Lebenspartner verstorben
        /// </summary>
        public const string FAMILYSTATE_TYPE_LV = "LV";
        /// <summary>
        /// Familienstand unbekannt
        /// </summary>
        public const string FAMILYSTATE_TYPE_FU = "FU";

        #endregion

        #region Static methods

        /// <summary>
        /// Gibt eine Liste der Familienstati zurück
        /// </summary>
        /// <param name="familyStateId">ID Wert in welcher in der Datenbank gefiltert werden soll</param>
        /// <param name="shortName">Kürzel für den Eintrag</param>
        /// <returns>Liste aller Familienstati</returns>
        public static IEnumerable<FamilyState> GetFamilyStates(int? familyStateId = null, string shortName = null)
        {
            var familyStates = GetFamilyStateList();

            if (familyStateId.HasValue)
                familyStates = familyStates.Where(f => f.FamilyStateID == familyStateId.Value);

            if (!string.IsNullOrEmpty(shortName))
                familyStates = familyStates.Where(f => f.ShortName == shortName);

            return familyStates;
        }

        /// <summary>
        /// Statische Methode zum einmaligen Laden aller Familienstati
        /// </summary>
        /// <returns>Liste der Familienstati</returns>
        private static IEnumerable<FamilyState> GetFamilyStateList()
        {
            if (_FamilyStates == null)
            {
                using (TafelModelContainer db = new TafelModelContainer())
                {
                    _FamilyStates = db.FamilyStates.ToList();
                }
            }
            return _FamilyStates;
        }

        #endregion
    }
}
