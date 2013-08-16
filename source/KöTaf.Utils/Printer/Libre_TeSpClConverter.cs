/**
 * Class: Libre_TeSpClConverter
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-05-08
 * 
 * Last modification: 2013-06-24 / Michael Müller
 */
using System;
using System.Diagnostics;
using CarlosAg.ExcelXmlWriter;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Management;
using System.Collections;


namespace KöTaf.Utils.Printer
{
    /// <summary>
    /// Klasse zur Erstellung von CALC - Sheets für den Druck.
    /// Abgehandelt werden: Sponsor, Kunden, Team, Statistik.
    /// Diese Klasse ist variable gehalten. Sämtliche Spalten sind 
    /// durch die Konfigurationsdatei einstellbar. Die dezeitige Einstellung
    /// ist passend auf die Daten abgestimmt.
    /// Die Farbe der Headers kann mit _HEADERCOLOR angepasst weden.
    /// 
    /// @note: Es existiert für das LIBRE_OFFICE ein VBA-Code welcher bei dem aktivieren 
    ///        des Dokuments geladen wird. Der Code selbst ist auch u.a. im Verzeichnis \src\LibreOffice\
    ///        zu finden.
    ///        Was macht der VBA CODE ?
    ///        a) Unterscheidung zwischen Spreadsheet und normales Dokument
    ///        b) Es dreht das Dokument (Hochformat / Querformat) falls notwendig.
    ///        c) Diese Notwendigkeit wird errechnet durch summieren der Spaltenbreite aller
    ///           beschriebenen Spalten
    ///        d) Der Code ist verantwortlich dafür das der HEADER auf jeder nachfolgenden Seite wieder
    ///           mitangezeigt wird.
    /// </summary>
    /// 

    class Libre_TeSpClConverter
    {
        private String _HEADERCOLOR = "#e6e6e6";
        private String _PATH_TO_CSV = Path.Combine(Environment.CurrentDirectory, "OO_CSV.csv");
        private List<String> _DataLine = new List<String>();
        Dictionary<String, int> dicDistance = new Dictionary<String,int>();
        private String _Filename =  Path.Combine(Environment.CurrentDirectory, "tmp.xml");
        private int _TmpNumber = 0,_POST_LANG, _POST_KURZ, _AKTIV, _GRUPPE, _ANR, _GEBURTSDATUM, _ADRESSE, _NAME, _STREET, _FUNKTION, _NATIONAL, _CITYNAME;
        private Workbook _Book;
        private Worksheet _Sheet;
        public String filename =  Path.Combine(Environment.CurrentDirectory, "tmp.xml");
        String sheetName;
        #region Contructor
        /// <summary>
        /// Konstuktor, hole alle relevanten Variablen-Werte aus der Konfigurationsdatei.
        /// </summary>
        /// <param name="sheetName"></param>
        public Libre_TeSpClConverter(PrintType sheetName){
            try
            {
                this._POST_KURZ = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "CTIYCODE_SHORT").ToLower());
                this._POST_LANG = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "CTIYCODE_LONG").ToLower());
                this._AKTIV = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "AKTIV").ToLower());
                this._GRUPPE = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "GROUP").ToLower());
                this._ANR = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "ANR").ToLower());
                this._GEBURTSDATUM = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "BIRTHDAY").ToLower());
                this._ADRESSE = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "ADRESS").ToLower());
                this._NAME = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "NAME").ToLower());
                this._STREET = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "STREET").ToLower());
                this._FUNKTION = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "FUNCTION").ToLower());
                this._NATIONAL = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "NATIONAL").ToLower());
                this._CITYNAME = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "FUNCTION").ToLower());
            }
            catch {
                throw new Exception("Fehler in der Konfiguarationsdatei");
            }
            switch (sheetName) { 
                case PrintType.Team:
                    this.sheetName = "T E A M L I S T E";
                    break;
                case PrintType.Sponsor:
                    this.sheetName = "S P O N S O R E N L I S T E";
                    break;
                case PrintType.Client:
                    this.sheetName = "K U N D E N L I S T E";
                    break;
                case PrintType.Statistic:
                    this.sheetName = "S T A T I S T I K";
                    break;
            }

            // Styles für die Kunden
            if (sheetName == PrintType.Client)
            {
                this.dicDistance.Add("PLZ", _POST_KURZ);
                this.dicDistance.Add("Aktiv", _AKTIV);
                this.dicDistance.Add("Gruppe", _GRUPPE);
                this.dicDistance.Add("Ausweis Nummer", _ANR);
                this.dicDistance.Add("Geburtsdatum", _GEBURTSDATUM);
                this.dicDistance.Add("Name", _NAME);
                this.dicDistance.Add("Nationalität", _NATIONAL);
                this.dicDistance.Add("Straße", _STREET);
                this.dicDistance.Add("Wohnort", _CITYNAME);
            }
            // Styles für die Statistik
            if (sheetName == PrintType.Statistic)
            {
                this.dicDistance.Add("Adresse", _ADRESSE);
            }
            // Styles für Team
            if (sheetName == PrintType.Team)
            {
                this.dicDistance.Add("Straße", _STREET);
                this.dicDistance.Add("Aktiv", _AKTIV);
                this.dicDistance.Add("Funktion", _FUNKTION);
                this.dicDistance.Add("Name", _NAME);
            }
            // Styles für Sponsor
            if (sheetName == PrintType.Sponsor)
            {
                this.dicDistance.Add("Name", _NAME);
                this.dicDistance.Add("Postleitzahl", _POST_LANG);
                this.dicDistance.Add("Aktiv", _AKTIV);
                this.dicDistance.Add("Straße",_STREET);

            }
            loadXLSFormatter(this.sheetName.ToString());
            // Lese die derzeitige CSV Datei aus, und fülle Daten in XML Datei.
            this.CSVReader();
            this.saveFileAndOpenOO();
        }
 
        #endregion       
        /// <summary>
        /// Lese Daten von der CSV Datei.
        /// </summary>
        #region Methods
        private void CSVReader()
        {
            int i = 0;
            int j = 0;
            foreach (string line in File.ReadAllLines(_PATH_TO_CSV, Encoding.Default))
            {
                string[] parts = line.Split(';');
                foreach (string part in parts)
                {
                    
                    letterCounter(part.ToCharArray().Length);
                    if (part == "True")
                    {
                        _DataLine.Add("JA");
                        j++;
                        continue;
                    }
                    if (part == "False")
                    {
                        _DataLine.Add("NEIN");
                        j++;
                        continue;
                    }
                    _DataLine.Add(part);
                    j++;
                }
                this.toXLS(i);
                i++;
            }
        }
        /// <summary>
        /// Formatiere Calc Dokument-Datei
        /// </summary>
        /// <param name="sheetName"></param>
        private void loadXLSFormatter(String sheetName)
        {
            _Book = new Workbook();
            _Book.ExcelWorkbook.WindowHeight = 9000;
            _Book.ExcelWorkbook.WindowWidth = 13860;
            _Book.ExcelWorkbook.WindowTopX = 240;
            _Book.ExcelWorkbook.WindowTopY = 75;
            _Book.ExcelWorkbook.ProtectWindows = false;
            _Book.ExcelWorkbook.ProtectStructure = false;

            WorksheetStyle style = _Book.Styles.Add("Header");
            WorksheetStyle dataStyle = _Book.Styles.Add("DataStyle");

            dataStyle.Borders.Add(StylePosition.Left, LineStyleOption.Continuous);
            dataStyle.Borders.Add(StylePosition.Right, LineStyleOption.Continuous); 
            dataStyle.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            dataStyle.Alignment.Vertical = StyleVerticalAlignment.Center;
            dataStyle.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous);
            style.Parent = "Normal";
            style.Font.Bold = true;
            style.Font.FontName = "Tahoma1";
            style.Font.Size = 11;
            style.Interior.Color = this._HEADERCOLOR;//"#e6e6ff";
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Interior.Pattern = StyleInteriorPattern.Solid;
            style.Alignment.WrapText = true;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
                  
            Worksheet sheet = _Book.Worksheets.Add(sheetName + "   vom   " + DateTime.Now.ToLongDateString());
            sheet.Options.Selected = true;
            sheet.Options.ProtectObjects = false;
            sheet.Options.ProtectScenarios = false;
            sheet.Options.PageSetup.Layout.Orientation = Orientation.Landscape;
            sheet.Options.PageSetup.Header.Margin = 0.492126F;
            sheet.Options.PageSetup.Footer.Margin = 0.492126F;
            sheet.Options.PageSetup.PageMargins.Bottom = 0.984252F;
            sheet.Options.PageSetup.PageMargins.Left = 0.7874016F;
            sheet.Options.PageSetup.PageMargins.Right = 0.7874016F;
            sheet.Options.PageSetup.PageMargins.Top = 0.984252F;
            sheet.Options.Print.PaperSizeIndex = 9;
            sheet.Options.Print.ValidPrinterInfo = true;
            this._Sheet = sheet;
        }

        /// <summary>
        /// Fülle XML-Dokument mit Daten
        /// </summary>
        /// <param name="level"></param>
        public void toXLS(int level)
        {
            WorksheetRow row = this._Sheet.Table.Rows.Add();
            if (level == 0)
            {
                row.Height = 30;
                for (int i = 0; i <= this._DataLine.Count - 1; i++)
                {
                    if(dicDistance.ContainsKey(this._DataLine[i].ToString()))
                    {
                        int valueOfdicDistance = dicDistance[this._DataLine[i].ToString()];
                        this._Sheet.Table.Columns.Add(new WorksheetColumn(valueOfdicDistance));
                    }
                    else
                        this._Sheet.Table.Columns.Add(new WorksheetColumn(110));
                    
                    row.Cells.Add(new WorksheetCell(this._DataLine[i].ToString(), "Header"));
                }
                this._DataLine.Clear();
            }
            if (level > 0)
            {
                if (level == 1)
                    level = 3;
                row.Index = level;
                for (int i = 0; i <= this._DataLine.Count - 1; i++)
                {
                    row.Cells.Add(this._DataLine[i].ToString(),DataType.String,"DataStyle");
                }
                this._DataLine.Clear();
            }
        }

        /// <summary>
        /// Spezialisiertes formatieren des Dokuments. 
        /// </summary>
        private void checkMySpreadSheet() {
            this._Sheet.Options.PageSetup.Layout.Orientation = Orientation.Landscape;
            int a = this._Sheet.Table.Rows.Count;
            int r = this._Sheet.Table.ExpandedColumnCount;
        }

        /// <summary>
        /// Speichere die generierte XML Datei ab.
        /// </summary>
        public void saveFileAndOpenOO()
        {

            List<String> list = new List<String>();
            Process[] pp = Process.GetProcessesByName("soffice.bin");
            foreach (Process p in pp)
            {
                // normal end
                p.CloseMainWindow();
                p.Kill(); 
                System.Threading.Thread.Sleep(2000);
            }
            _Book.Save(_Filename);
            string pfad = Utils.Parser.RegistryParser.getLibreOfficeInstallPath() + "\\scalc.exe";
            string argument = _Filename;
            Process.Start(pfad, argument);
            
        }
        /// <summary>
        /// Lese grösste Zeichenkette an Buchstaben ein. 
        /// </summary>
        /// <param name="letter"></param>
        private void letterCounter(int letter){
            if (letter > this._TmpNumber)
                this._TmpNumber = letter;
        }
    }
}
#endregion