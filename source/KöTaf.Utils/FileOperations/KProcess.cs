using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace KöTaf.Utils.FileOperations
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Spezieller Prozess mit erweiterten Funktionen
    /// Kann über Threads gestartet werden
    /// </summary>
    public class KProcess : Process
    {
        string file;

        /// <summary>
        /// Erzeugt einen neuen Prozess
        /// </summary>
        /// <param name="path">Pfad zum ausführbaren Programm</param>
        /// <param name="argument">Parameter, z.B. Dateipfad</param>
        /// <param name="openReadOnly">schreibgeschützt öffnen (LibreOffice)</param>
        /// <param name="deleteDocAfterClose">Dokument nach dem Beenden löschen</param>
        public KProcess(string path, string argument, bool openReadOnly, bool deleteDocAfterClose = false)
            : base()
        {
            try
            {
                string currentDir = System.IO.Directory.GetCurrentDirectory();
                
                // Ersetze Platzhalter aus Konfigurationsdatei
                string path2 = path.Replace("%PROGRAMPATH%", currentDir);
                string argumentR = argument.Replace("%PROGRAMPATH%", currentDir);

                if (!File.Exists(path2))
                    throw new Exception();

                StartInfo.FileName = path2;
                StartInfo.Arguments = (openReadOnly ? "--view " : "") + argumentR;  // schreibgeschützt
                file = argumentR;
                EnableRaisingEvents = true;
                if (deleteDocAfterClose)
                    Exited += new EventHandler(deleteTempFileAfterProcessClosed);
                Start();
            }
            catch
            {
                try
                {
                    // Datei löschen
                    File.Delete(file);
                }
                catch { }
            }
        }

        /// <summary>
        /// Funktion, die beim Beenden des Prozesses aufgerufen wird
        /// Löscht die geöffnete Datei
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteTempFileAfterProcessClosed(object sender, System.EventArgs e)
        {
            try
            {
                File.Delete(this.file);
            }
            catch
            {
            }
        }
    }
}
