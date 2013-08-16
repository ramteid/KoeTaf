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
    public partial class ColumnAssignment
    {
        /// <summary>
        /// Gibt alle ColumnAssignments zurück
        /// </summary>
        /// <param name="columnAssignmentID">Nach welcher ID in der Datenbank gefiltert werden soll</param>
        /// <returns>Liste aller ColumnAssignments</returns>
        public static IEnumerable<ColumnAssignment> GetColumnAssignments(int? columnAssignmentID = null, int? formletterPatternID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var columnAssignments = db.ColumnAssignments
                                            .Include("FormletterPattern")
                                            .AsQueryable();

                if (columnAssignmentID.HasValue)
                    columnAssignments = columnAssignments.Where(ca => ca.ColumnAssignmentID == columnAssignmentID.Value);

                if (formletterPatternID.HasValue)
                    columnAssignments = columnAssignments.Where(ca => ca.FormletterPattern.FormletterPatternID == formletterPatternID.Value);

                return columnAssignments.ToList();
            }
        }

        /// <summary>
        /// Fügt ein neues ColumnAssignment in der Datenbank hinzu
        /// </summary>
        /// <param name="csvColumn">Die CSV Spalte</param>
        /// <param name="databaseTable">Datenbank Tabelle</param>
        /// <param name="databaseColumn">Datenbank Spalte</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int formletterPatternID, string csvColumn, string databaseTable, string databaseColumn)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var columnAssignment = new ColumnAssignment()
                {
                    FormletterPattern = db.FormletterPatterns.Single(fp => fp.FormletterPatternID == formletterPatternID),
                    CsvColumn = csvColumn,
                    DatabaseTable = databaseTable,
                    DatabaseColumn = databaseColumn
                };

                db.ColumnAssignments.AddObject(columnAssignment);
                db.SaveChanges();

                return columnAssignment.ColumnAssignmentID;
            }
        }

        /// <summary>
        /// Löscht einen bestehenden Eintrag in der Datenbank
        /// </summary>
        /// <param name="columnAssignmentID"></param>
        public static void Delete(int columnAssignmentID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var columnAssignment = db.ColumnAssignments.Single(ca => ca.ColumnAssignmentID == columnAssignmentID);

                db.ColumnAssignments.DeleteObject(columnAssignment);
                db.SaveChanges();
            }
        }
    }
}
