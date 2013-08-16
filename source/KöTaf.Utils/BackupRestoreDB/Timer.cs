/**
 * Class: Timer
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-06-05
 * 
 * Last modification: 2013-06-18 / Michael Müller
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace KöTaf.Utils.BackupRestore
{
    /// <summary>
    /// Prüfung auf Gültigkeit der Kötaf Restore - Dateien
    /// Die Klasse hat auch die Löschfunktion der Restore Dateien.
    /// Die Löschfunktion ist Variabel gehalten, und somit in der Konfigurationsdatei
    /// einstellbar (Tage).
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        ///
        private int _TIME_TO_DELETE = 365;
        private String _Warning = "Fehler in der Konfigurationsdatei (TIME_TO_DELETE)";
        #region Contructor
        public Timer() {
            
            try
            {
                this._TIME_TO_DELETE = Convert.ToInt32(Parser.IniParser.GetSetting("RESTORE_TIMER", "DAYS_UNTIL_DELETE_FILES"));
            }
            catch {
                throw new Exception(_Warning);
            }
            if (this._TIME_TO_DELETE == 0) {
                throw new Exception(_Warning);
            }
                 
        }
        #endregion
        #region Methods
        /// <summary>
        /// Falls Datei zu alt >= TIME_TO_DELETE , lösche Datei.
        /// </summary>
        /// <param name="fileDate"></param>
        /// <returns></returns>
        public bool deleteBackup(DateTime fileDate) { 
        DateTime currentDate = DateTime.Now.Date;
        System.TimeSpan result = currentDate.Subtract(fileDate);
        if (result.Days >= this._TIME_TO_DELETE)
            return false;
        return true;
        }
        /// <summary>
        /// Methode um den TimeStamp für das Backup zu realisieren.
        /// Diese Methode liest aus der XML Datei letzte Backup Datum heraus.
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public List<String> readXML(String xmlFileName, String xmlRootChild, String childElem1, String childElem2 )
        {
            if ((!File.Exists(xmlFileName))) {
                createTimeStampXML(xmlFileName);
            }
            List<String> liste = new List<String>();
            XElement xElement = XElement.Load(xmlFileName);
            var timer =
            from xVar in xElement.Descendants(xmlRootChild)
            select new
            {
                date = xVar.Element(childElem1).Value,
                iswrite = xVar.Element(childElem2).Value
            };
            foreach (var item in timer)
            {
                liste.Add(item.date);
                liste.Add(item.iswrite);
            }
            return liste;
        }

        /// <summary>
        /// Schreibe XML Datei für den TimeStamp. 
        /// </summary>
        /// <param name="pathToXML"></param>
        /// <param name="isWriteItem"></param>
        /// <param name="dateItem"></param>
        /// <param name="singleNodePath1"></param>
        /// <param name="singleNodePath2"></param>
        public void writeToXML(String pathToXML, String isWriteItem, String dateItem, String singleNodePath1, String singleNodePath2)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.NewLineOnAttributes = true;
            xmlWriterSettings.Indent = true;
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(pathToXML))
            {
                //Lege neue XML Datei an
                createTimeStampXML(pathToXML);
            }
                xmlDoc.Load(pathToXML);
                XmlNode nodeDate = xmlDoc.SelectSingleNode(singleNodePath1);
                nodeDate.InnerText = dateItem;
                XmlNode nodeIsWrite = xmlDoc.SelectSingleNode(singleNodePath2);
                nodeIsWrite.InnerText = isWriteItem;
                xmlDoc.Save(pathToXML);
        }
        /// <summary>
        /// Erstelle die TimeStamp Datei, falls notwendig / falls nicht existiert.
        /// </summary>
        /// <param name="pathToXML"></param>
        private static void createTimeStampXML(String pathToXML)
        {
            XmlDocument XD = new XmlDocument();
            XmlNode Root = XD.AppendChild(XD.CreateElement("timeStamp"));
            XmlNode backup = Root.AppendChild(XD.CreateElement("backup"));
            XmlNode date = backup.AppendChild(XD.CreateElement("date"));
            XmlNode isWrite = backup.AppendChild(XD.CreateElement("iswrite"));
            date.InnerText = DateTime.Now.ToShortDateString();
            isWrite.InnerText = "False";
            XD.Save(pathToXML);
        }
    }
}
#endregion