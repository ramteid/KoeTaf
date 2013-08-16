/**
 * Class: BackupAndRestoreDatabase
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-05
 * 
 * Last modification: 2013-06-20 / Michael Müller
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Collections;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using KöTaf.DataModel;
using KöTaf.Utils.Database;
using KöTaf.Utils.Parser;

namespace KöTaf.Utils.BackupRestore
{
    /// <summary>
    /// Diese Klasse führt das Backup und die Wiederherstellung der Datenbank durch.
    /// </summary>
    /// 
    public class BackupAndRestoreDatabase
    {   
        private String backupFileName;
        private String backupDirectory;
        public String _PATH_TO_CURRENT_DB =
                      Path.Combine(Environment.CurrentDirectory,"TafelDB.sdf");
        private String _FILENAME_DB_EXTENSION; 
        /// <summary>
        /// Constructor
        /// Rufe relevante Daten aus der Konfigurationsdatei ab.
        /// </summary>
        #region Contructor
        public BackupAndRestoreDatabase() {
            this._FILENAME_DB_EXTENSION = "." + IniParser.GetSetting("USB", "FILENAME_DB_EXTENSION").ToLower();
                }
        #endregion

#region Methods
        /// <summary>
        /// Eigentlicher Startprozess des Backup / Restores; Eingangsvariablen validieren
        /// </summary>
        /// <param name="level"></param>
        /// <param name="filePath"></param>
        /// 
        public void StartBR(String level = "", String filePath = "")
        {   

            this.backupFileName = Path.GetFileName(filePath);
            this.backupDirectory = Path.GetDirectoryName(filePath);
            if (this.backupDirectory != "" && (level == "backupDB" || level == "restoreDB"))
            {
                _Menu(level);
            }
            else
                throw new Exception();
        }

        /// <summary>
        /// Zwischen Backup und Wiederhertellung unterscheiden
        /// </summary>
        /// <param name="level"></param>
        private void _Menu(String level)
        {
            switch (level)
            {
                case "backupDB":
                    BackupDB();
                    break;
                case "restoreDB":
                    if (_DatabaseValidator())
                    {
                        _RestoreDB();
                    }
                    else
                    { throw new Exception("Die von Ihnen ausgewählte Datei enthält ungültige Werte. Vrogang wird abgebrochen."); }
                    break;
            }
        }

        /// <summary>
        /// Methode für das Backup
        /// </summary>
        private void BackupDB()
        {
            if (!Directory.Exists(this.backupDirectory)){
                System.IO.Directory.CreateDirectory(this.backupDirectory);
            }
            File.Copy(this._PATH_TO_CURRENT_DB, Path.Combine(this.backupDirectory, "Backup_vom_" + DateTime.Now.ToLongDateString()) + _FILENAME_DB_EXTENSION, true);
        }

        /// <summary>
        /// Methode für die Wiederherstellung
        /// </summary>
        private void _RestoreDB()
        {
            //CLOSE CURRENT DB !!!!
            File.Copy(Path.Combine(this.backupDirectory, this.backupFileName), this._PATH_TO_CURRENT_DB, true);
            //OPEN CURRENT DB  !!!!
        }

        /// <summary>
        /// Wiederzuherstellende Daten mit der derzeitigen Datenbank validieren Teil1
        /// /// </summary>
        /// <returns></returns>
        private bool _DatabaseValidator()
        {
            List<String> sourceDatabase = new List<string>();
            var objectListCurrentDatabase = KöTaf.DataModel.Utils.DatabaseUtils.GetCurrentDatabaseTables();
            foreach (var elem in objectListCurrentDatabase)
            {
                sourceDatabase.Add(elem.Name);
            }
            String[] destinationDatabase = Tools.GetFileDatabaseTables(Path.Combine(this.backupDirectory, this.backupFileName)).ToArray();
            int table = sourceDatabase.Count;
            bool result = _CheckForEqualDB(sourceDatabase.ToArray(), destinationDatabase);
            return true;
        }

        /// <summary>
        /// Wiederzuherstellende Daten mit der derzeitigen Datenbank validieren Teil2
        /// </summary>
        /// <typeparam name="String"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private bool _CheckForEqualDB<String>(String[] a1, String[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            EqualityComparer<String> comparer = EqualityComparer<String>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
    }
}
#endregion