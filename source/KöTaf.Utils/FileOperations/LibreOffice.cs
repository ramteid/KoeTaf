using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using KöTaf.Utils.Parser;
using System.Text.RegularExpressions;
using Ionic.Zip;
using System.Xml;

namespace KöTaf.Utils.FileOperations
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Diese Klasse bietet LibreOffice-spezifische Funktionen
    /// </summary>
    public class LibreOffice
    {
        /// <summary>
        /// Öffnet ein angegebene Datei mit OpenOffice Writer
        /// </summary>
        /// <param name="fileName">Dateiname</param>
        /// <param name="openReadOnly">schreibgeschützt öffnen</param>
        /// <param name="deleteDocAfterClose">angegebene Datei nach Beendigung löschen</param>
        public static void openWithWriter(string fileName, bool openReadOnly = true, bool deleteDocAfterClose = false)
        {
            string path = RegistryParser.getLibreOfficeInstallPath() + "\\swriter.exe";

            KProcess process = new KProcess(path, fileName, openReadOnly, deleteDocAfterClose);
        }

        /// <summary>
        /// Kopiert die angegebene Datei zu einer temporären Datei
        /// </summary>
        /// <param name="filePath">zu kopierende Datei</param>
        /// <param name="copiedFilePath">Referenz zum Pfad der Kopie</param>
        /// <returns>Erfolg</returns>
        public static bool copyFileToTempFile(string filePath, out string copiedFilePath)
        {
            try
            {
                String matchpattern = @"[^\\]*\.odt$";
                String replacementpattern = @"_tmp.odt";
                string tmpFilePath = Regex.Replace(filePath, matchpattern, replacementpattern);

                File.Copy(filePath, tmpFilePath, true);
                copiedFilePath = tmpFilePath;

                return true;
            }
            catch
            {
                copiedFilePath = "";
                return false;
            }
        }

        /// <summary>
        /// Prüft ob LibreOffice installiert ist
        /// </summary>
        /// <returns>Installationsstatus</returns>
        public static bool isLibreOfficeInstalled()
        {
            return !(RegistryParser.getLibreOfficeInstallPath().Equals(""));
        }

        /// <summary>
        /// Liest die Werte von einem Attributs eines Datenquellen-verknüpften Feldes aus einer OpenDocument 1.2-basierter Datei aus
        /// mögliche Attribute: text:table-name, text:column-name, text:database-name, text:table-type
        /// </summary>
        /// <param name="filePath">auszulesende Datei</param>
        /// <param name="attribute">auszulesendes Attribut</param>
        /// <returns>Liste von Strings</returns>
        public static IEnumerable<string> GetDatabaseFieldAttributeFromODT(string filePath, string attribute)
        {
            IList<string> attributeValues = new List<string>();

            try
            {
                using (ZipFile zip = ZipFile.Read(filePath))
                {
                    using (var stream = zip["content.xml"].OpenReader())
                    {
                        using (var content = new StreamReader(stream))
                        {
                            using (var xmlReader = new XmlTextReader(content))
                            {
                                while (xmlReader.Read())
                                {
                                    if (xmlReader.NodeType == XmlNodeType.Element)             // check for XML node
                                        if (xmlReader.Name == "text:database-display")         // this is the tag indicating a link to an db/csv value
                                            while (xmlReader.MoveToNextAttribute())            // read its attributes
                                                if (xmlReader.Name == attribute)               // check if it is the attribute we are looking for
                                                    attributeValues.Add(xmlReader.Value);
                                }

                                return attributeValues;
                            }
                        }
                    }
                }
            }
            catch
            {
                return attributeValues;
            }
        }

        /// <summary>
        /// Ersetzt in einem ODT-Dokument-XML bestimmte Zeichenfolgen
        /// Kopiert dazu die Datei in eine temporäre, bearbeitbare Datei
        /// </summary>
        /// <param name="filePath">zu bearbeitende Datei</param>
        /// <param name="toReplace">Liste von Such-Strings</param>
        /// <param name="replacements">Liste von Ersetzen-Strings</param>
        /// <param name="tmpFilePath">Pfad zur temporären Datei</param>
        /// <returns>Erfolg</returns>
        public static bool replaceXMLstringInODT(string filePath, List<string> toReplace, List<string> replacements, out string tmpFilePath)
        {
            // toReplace muss gleich viele Elemente gespeichert haben wie replacements
            if (toReplace.Count != replacements.Count)
            {
                tmpFilePath = "";
                return false;
            }

            if (!(LibreOffice.copyFileToTempFile(filePath, out tmpFilePath)))
                return false;

            try
            {
                using (ZipFile zip = ZipFile.Read(tmpFilePath))
                {
                    string sContent;

                    using (var stream = zip["content.xml"].OpenReader())
                    {
                        using (var content = new StreamReader(stream))
                        {
                            sContent = content.ReadToEnd();
                        }
                        for (int i = 0; i < toReplace.Count; i++)
                            sContent = sContent.Replace(toReplace[i], replacements[i]);
                    }
                    zip.RemoveEntry("content.xml");

                    string tmpFileName = "content.xml";

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@tmpFileName, false, System.Text.Encoding.UTF8))
                    {
                        file.Write(sContent);
                    }
                    zip.AddFile(tmpFileName);
                    zip.Save();
                    File.Delete(tmpFileName);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
