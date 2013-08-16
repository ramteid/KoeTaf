using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace KöTaf.Utils.FileOperations
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Enthält Funktionen für Dateioperationen
    /// </summary>
    public class CheckFileDir
    {
        /// <summary>
        /// Prüft ob Datei existiert
        /// </summary>
        /// <param name="filePath">Dateipfad</param>
        /// <returns>Existenz</returns>
        public static bool checkIfFileExists(string filePath)
        {
            try
            {
                return System.IO.File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prüft ob Verzeichnis existiert
        /// </summary>
        /// <param name="dirPath">Verzeichnis</param>
        /// <returns>Existenz</returns>
        public static bool checkIfDirectoryExists(string dirPath)
        {
            try
            {
                return System.IO.Directory.Exists(dirPath);
            }
            catch
            {
                return false;
            }
        }

    }
}
