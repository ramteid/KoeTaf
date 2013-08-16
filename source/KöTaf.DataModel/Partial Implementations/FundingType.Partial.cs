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
    public partial class FundingType
    {
        /// <summary>
        /// Statische Liste mit allen Finanztypen
        /// </summary>
        private static IEnumerable<FundingType> _FundingTypes;

        #region Static Methods

        /// <summary>
        /// Gibt alle Finanztypen zurück
        /// </summary>
        /// <param name="fundingTypeID">Filterung nach ID</param>
        /// <returns>Liste aller Finanztypen</returns>
        public static IEnumerable<FundingType> GetFundingTypes(int? fundingTypeID = null)
        {
            var fundingTypes = GetFundingTypeList();

            if (fundingTypeID.HasValue)
                fundingTypes = fundingTypes.Where(f => f.FundingTypeID == fundingTypeID.Value);

            return fundingTypes.ToList();
        }

        /// <summary>
        /// Statische Klasse um mehrfaches Laden aus der Datenbank zu verhindern
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<FundingType> GetFundingTypeList()
        {
            if (_FundingTypes == null)
            {
                using (TafelModelContainer db = new TafelModelContainer())
                {
                    _FundingTypes = db.FundingTypes.ToList();
                }
            }
            return _FundingTypes;
        }

        /// <summary>
        /// Fügt einen neuen finanziellen Typ der Datenbank hinzu
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int Add(string name)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var fundingType = new FundingType { Name = name };

                db.FundingTypes.AddObject(fundingType);
                db.SaveChanges();

                return fundingType.FundingTypeID;
            }
        }

        #endregion
    }
}
