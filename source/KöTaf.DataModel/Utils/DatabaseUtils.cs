using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel.Utils
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Klasse: Datenbank Tools
    /// </summary>
    public class DatabaseUtils
    {
        #region static methods

        /// <summary>
        /// Gibt eine Liste aller Tabellen und Spalten der aktuellen Datenbank zurück
        /// </summary>
        /// <returns></returns>
        public static IList<DatabaseTable> GetCurrentDatabaseTables()
        {
            // Url: http://www.mycsharp.de/wbb2/thread.php?threadid=26610
            using (TafelModelContainer db = new TafelModelContainer())
            {
                IList<DatabaseTable> tableList = new List<DatabaseTable>();

                var tables = db.ExecuteStoreQuery<string>("SELECT table_name FROM INFORMATION_SCHEMA.TABLES");
                foreach (var name in tables)
                {
                    string sqlCmd = string.Format("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'", name);
                    var columns = db.ExecuteStoreQuery<string>(sqlCmd);
                    tableList.Add(new DatabaseTable
                    {
                        Name = name,
                        Columns = columns.ToList()
                    });
                }

                return tableList;
            }
        }

        #endregion

        #region inner classes

        /// <summary>
        /// (c) Florian Wasielewski
        /// 
        /// Klasse: Repräsentiert eine Datenbank Tabelle
        /// </summary>
        public class DatabaseTable
        {
            /// <summary>
            /// Name der Datenbank Tabelle
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Liste aller Spalten in der Tabelle
            /// </summary>
            public IEnumerable<string> Columns { get; set; }
        }

        #endregion

        #region static properties

        /// <summary>
        /// Gibt einen Wert zurück ob die derzeitige Datenbank existiert
        /// </summary>
        public static bool IsCurrentDatabaseExist
        {
            get
            {
                using (TafelModelContainer db = new TafelModelContainer())
                {
                    return db.DatabaseExists();
                }
            }
        }

        #endregion
    }
}
