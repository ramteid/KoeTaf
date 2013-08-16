using System;
using System.IO;
using System.Collections;

namespace KöTaf.Utils.Parser
{
    /// <summary>
    /// Author: Dietmar Sach
    /// INI-Parser für die Konfigurationsdatei config.ini
    /// </summary>
    public class IniParser
    {
        private static Hashtable keyPairs = new Hashtable();
        private static String iniFilePath;

        private struct SectionPair
        {
            public String Section;
            public String Key;
        }

        /// <summary>
        /// Öffnet die INI-Datei und zählt die Werte im IniParser auf
        /// </summary>
        static IniParser()
        {
            string iniPath = Path.Combine(Environment.CurrentDirectory, "config.ini");
            TextReader iniFile = null;
            String strLine = null;
            String currentRoot = null;
            String[] keyPair = null;

            iniFilePath = iniPath;

            if (File.Exists(iniPath))
            {
                try
                {
                    iniFile = new StreamReader(iniPath, System.Text.Encoding.Default);

                    strLine = iniFile.ReadLine();

                    while (strLine != null)
                    {
                        strLine = strLine.Trim();

                        if (strLine != "")
                        {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                            {
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            }
                            else
                            {
                                if (strLine.StartsWith("'"))
                                {
                                    // Kommentarzeile
                                }
                                else
                                {
                                    keyPair = strLine.Split(new char[] { '=' }, 2);
                                    SectionPair sectionPair;
                                    String value = null;

                                    if (currentRoot == null)
                                        currentRoot = "ROOT";

                                    sectionPair.Section = currentRoot;
                                    sectionPair.Key = keyPair[0];

                                    if (keyPair.Length > 1)
                                        value = keyPair[1];

                                    keyPairs.Add(sectionPair, value);
                                }
                            }
                        }

                        strLine = iniFile.ReadLine();
                    }

                }
                catch
                {
                    throw new IOException();
                }
                finally
                {
                    if (iniFile != null)
                        iniFile.Close();
                }
            }
            else
                throw new FileNotFoundException(iniPath);

        }

        /// <summary>
        /// Gibt den Wert für die angegebene Sektion zurück, Schlüsselpaar
        /// </summary>
        /// <param name="sectionName">Sektion</param>
        /// <param name="settingName">Schlüssel</param>
        public static String GetSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;

            return (String)keyPairs[sectionPair];
        }

        /// <summary>
        /// Zählt alle Zeilen für die angegebene Sektion auf
        /// </summary>
        /// <param name="sectionName">aufzuzählende Sektion</param>;
        public static String[] EnumSection(String sectionName)
        {
            ArrayList tmpArray = new ArrayList();

            foreach (SectionPair pair in keyPairs.Keys)
            {
                if (pair.Section == sectionName)
                    tmpArray.Add(pair.Key);
            }

            return (String[])tmpArray.ToArray(typeof(String));
        }

        /// <summary>
        /// Ersetzt oder fügt eine neue Einstellung der zu sichernden Tabelle hinzu
        /// </summary>
        /// <param name="sectionName">Übergeordnete Sektion<;/param>
        /// <param name="settingName">zu speichernden Schlüsselname</param>
        /// <param name="settingValue">Schlüsselwert</param>
        public static void AddSetting(String sectionName, String settingName, String settingValue)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;

            if (keyPairs.ContainsKey(sectionPair))
                keyPairs.Remove(sectionPair);

            keyPairs.Add(sectionPair, settingValue);
        }

        /// <summary>
        /// Ersetzt oder fügt eine Einstellung zu der zu speichernden Tabelle hinzu als null-Wert
        /// </summary>
        /// <param name="sectionName">Section to add under.<;/param>
        /// <param name="settingName">Key name to add.</param>
        public static void AddSetting(String sectionName, String settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        /// <summary>
        /// Eine Einstellung entfernen
        /// </summary>
        /// <param name="sectionName">Übergeordnete Sektion<;/param>
        /// <param name="settingName">zu entfernender Schlüssel</param>
        public static void DeleteSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName;
            sectionPair.Key = settingName;

            if (keyPairs.ContainsKey(sectionPair))
                keyPairs.Remove(sectionPair);
        }

        /// <summary>
        /// Speichere Einstellungen in eine neue Datei
        /// </summary>
        /// <param name="newFilePath">Neuer Dateipfad</param>
        public static void SaveSettings(String newFilePath)
        {
            ArrayList sections = new ArrayList();
            String tmpValue = "";
            String strToSave = "";

            foreach (SectionPair sectionPair in keyPairs.Keys)
            {
                if (!sections.Contains(sectionPair.Section))
                    sections.Add(sectionPair.Section);
            }

            foreach (String section in sections)
            {
                strToSave += ("[" + section + "]\r\n");

                foreach (SectionPair sectionPair in keyPairs.Keys)
                {
                    if (sectionPair.Section == section)
                    {
                        tmpValue = (String)keyPairs[sectionPair];

                        if (tmpValue != null)
                            tmpValue = "=" + tmpValue;

                        strToSave += (sectionPair.Key + tmpValue + "\r\n");
                    }
                }

                strToSave += "\r\n";
            }

            try
            {
                TextWriter tw = new StreamWriter(newFilePath);
                tw.Write(strToSave);
                tw.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Schreibe Einstellungen in die Ini-Datei zurück
        /// </summary>
        public static void SaveSettings()
        {
            SaveSettings(iniFilePath);
        }
    }
}