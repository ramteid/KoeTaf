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
    public partial class FormletterPattern
    {
        /// <summary>
        /// Gibt alle Serienbrief Muster zurück
        /// </summary>
        /// <param name="formletterPatternID">Die ID nach welchem Serienbrief gefiltert werden soll</param>
        /// <returns>Liste alle Serienbrief Muster</returns>
        public static IEnumerable<FormletterPattern> GetFormletterPatterns(int? formletterPatternID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var formletterPatterns = db.FormletterPatterns
                                            .Include("ColumnAssignment")
                                            .Include("FilterSet")
                                            .AsQueryable();

                if (formletterPatternID.HasValue)
                    formletterPatterns = formletterPatterns.Where(fp => fp.FormletterPatternID == formletterPatternID.Value);

                return formletterPatterns.ToList();
            }
        }

        /// <summary>
        /// Methode zum hinzufügen eines neuen Serienbrief Musters
        /// </summary>
        /// <param name="filterSetID">Die FilterSet ID in der Datenbank</param>
        /// <param name="columnAssignmentID">Die ColumnAssignment ID in der Datenbank</param>
        /// <param name="name">Name des Musters</param>
        /// <param name="saluationM"></param>
        /// <param name="saluationF"></param>
        /// <param name="saluationNT"></param>
        /// <param name="filename">Dateiname des Serienbriefs</param>
        /// <param name="text">Text des Serienbriefs</param>
        /// <returns></returns>
        public static int Add(string name, string saluationM, string saluationF, string saluationNT, string filename, string text)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var formletterPattern = new FormletterPattern()
                {
                    Name = name,
                    SaluationM = saluationM,
                    SaluationF = saluationF,
                    SaluationNT = saluationNT,
                    FileName = filename,
                    Text = text
                };

                db.FormletterPatterns.AddObject(formletterPattern);
                db.SaveChanges();

                return formletterPattern.FormletterPatternID;
            }
        }

        /// <summary>
        /// Methode zum löschen eines Eintrags in der Datenbank
        /// </summary>
        /// <param name="formletterPatternID">Die ID des Serienbrief Musters</param>
        public static void Delete(int formletterPatternID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var formletterPattern = db.FormletterPatterns.Single(fp => fp.FormletterPatternID == formletterPatternID);

                db.FormletterPatterns.DeleteObject(formletterPattern);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Der Name repräsentiert den String der partiellen Klasse
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
